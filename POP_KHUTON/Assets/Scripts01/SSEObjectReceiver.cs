using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json; // Requires Newtonsoft.Json package/library
using System.Net.Http;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System;
using UnityEngine.PlayerLoop; // Required for Action

/// <summary>
/// Data structure for an individual detected object, matching the server's JSON format.
/// The server sends a dictionary where keys are sector strings (e.g., "0-0")
/// and values are lists of these NewDetectedObjectInfo objects.
/// </summary>
[System.Serializable]
public class NewDetectedObjectInfo
{
    public int sector_row;
    public int sector_col;

    // Uses JsonProperty to map JSON key "Lv" to C# field "Level".
    // C# convention is PascalCase for public members.
    [JsonProperty("Lv")]
    public string Level;

    // Uses JsonProperty to map JSON key "type" to C# field "ObjectType".
    [JsonProperty("type")]
    public string ObjectType; // "Type" can be ambiguous with System.Type
}

public class SSEObjectReceiver : MonoBehaviour
{
    [Header("Server Configuration")]
    public string serverStreamUrl = "http://localhost:8000/detection_stream"; // FastAPI SSE endpoint

    [Header("Connection Settings")]
    public float reconnectDelaySeconds = 5.0f; // Delay before attempting to reconnect

    private HttpClient httpClient;
    private CancellationTokenSource cancellationTokenSource;
    private bool isTryingToConnect = false; // To prevent multiple concurrent connection attempts

    void Start()
    {
        // Ensure the dispatcher instance is available or created.
        // This line is crucial for marshalling calls to the main thread.
        if (UnityMainThreadDispatcher.Instance() == null)
        {
            Debug.LogError("UnityMainThreadDispatcher instance could not be initialized. SSEObjectReceiver may not function correctly.");
            return;
        }

        httpClient = new HttpClient();
        // Set a very long timeout because SSE connections are meant to be long-lived.
        httpClient.Timeout = Timeout.InfiniteTimeSpan;

        // Start the connection process.
        AttemptSseConnection();
    }

    void AttemptSseConnection()
    {
        if (isTryingToConnect) return; // Already attempting a connection

        isTryingToConnect = true;
        if (cancellationTokenSource != null) // Clean up previous token source if any
        {
            cancellationTokenSource.Cancel();
            cancellationTokenSource.Dispose();
        }
        cancellationTokenSource = new CancellationTokenSource();

        Debug.Log($"[SSEObjectReceiver] Attempting to connect to SSE stream: {serverStreamUrl}");
        // Run ConnectToSseStream asynchronously.
        // We don't await it here directly in Start/AttemptSseConnection
        // because these are not async methods. The Task runs in the background.
        Task.Run(() => ConnectToSseStream(cancellationTokenSource.Token), cancellationTokenSource.Token);
    }


    private async Task ConnectToSseStream(CancellationToken cancellationToken)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, serverStreamUrl);
            // Server should send "text/event-stream"
            request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("text/event-stream"));

            using (HttpResponseMessage response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken))
            {
                response.EnsureSuccessStatusCode(); // Throws an exception if not successful
                Debug.Log("[SSEObjectReceiver] Successfully connected to SSE stream. Waiting for data...");
                isTryingToConnect = false; // Successfully connected, reset flag

                using (var stream = await response.Content.ReadAsStreamAsync()) // Consider ReadAsStreamAsync(cancellationToken) in .NET 6+
                using (var reader = new StreamReader(stream))
                {
                    while (!reader.EndOfStream)
                    {
                        if (cancellationToken.IsCancellationRequested)
                        {
                            Debug.Log("[SSEObjectReceiver] Stream reading cancelled.");
                            break;
                        }

                        string line = await reader.ReadLineAsync(); // Consider ReadLineAsync(cancellationToken) in .NET 6+

                        if (string.IsNullOrWhiteSpace(line) || !line.StartsWith("data:"))
                        {
                            continue; // Skip empty lines or lines not starting with "data:"
                        }

                        string jsonData = line.Substring("data:".Length).Trim();
                        // For debugging, can be very verbose:
                        // Debug.Log($"[SSEObjectReceiver] Received SSE data line: {jsonData}");

                        // Enqueue the processing to be done on Unity's main thread
                        UnityMainThreadDispatcher.Instance().Enqueue(() =>
                        {
                            try
                            {
                                Dictionary<string, List<NewDetectedObjectInfo>> detectionData =
                                    JsonConvert.DeserializeObject<Dictionary<string, List<NewDetectedObjectInfo>>>(jsonData);

                                if (detectionData != null)
                                {
                                    UpdateDetectionInfoInUnity(detectionData);
                                }
                                else
                                {
                                    Debug.LogWarning("[SSEObjectReceiver] Deserialized detection data is null.");
                                }
                            }
                            catch (JsonException ex)
                            {
                                Debug.LogError($"[SSEObjectReceiver] JSON Deserialization Error: {ex.Message}\nJSON: {jsonData}");
                            }
                            catch (Exception ex) // Catch any other exceptions during processing
                            {
                                Debug.LogError($"[SSEObjectReceiver] Error processing received data on main thread: {ex.Message}\nStackTrace: {ex.StackTrace}");
                            }
                        });
                    }
                }
            }
        }
        catch (HttpRequestException ex)
        {
            if (!cancellationToken.IsCancellationRequested)
            {
                Debug.LogError($"[SSEObjectReceiver] SSE Connection Error (HttpRequestException): {ex.Message}. URL: {serverStreamUrl}");
            }
        }
        catch (TaskCanceledException) // Expected when cancellationTokenSource.Cancel() is called
        {
            Debug.Log("[SSEObjectReceiver] SSE stream reading task was canceled.");
        }
        catch (Exception ex) // Catch-all for other unexpected errors
        {
            if (!cancellationToken.IsCancellationRequested)
            {
                Debug.LogError($"[SSEObjectReceiver] SSE Stream Error (General Exception): {ex.GetType().Name} - {ex.Message}\nStackTrace: {ex.StackTrace}");
            }
        }
        finally
        {
            isTryingToConnect = false; // Reset flag regardless of outcome
            if (!cancellationToken.IsCancellationRequested && Application.isPlaying) // Check if still playing and not intentionally cancelled
            {
                Debug.Log($"[SSEObjectReceiver] SSE connection lost or ended. Attempting to reconnect in {reconnectDelaySeconds} seconds...");
                // Use a coroutine for delayed reconnection to avoid issues with async void from finally
                UnityMainThreadDispatcher.Instance().Enqueue(() => StartCoroutine(DelayedReconnect()));
            }
            else if (cancellationToken.IsCancellationRequested)
            {
                Debug.Log("[SSEObjectReceiver] SSE Task was cancelled. No auto-reconnect.");
            }
        }
    }

    System.Collections.IEnumerator DelayedReconnect()
    {
        yield return new WaitForSeconds(reconnectDelaySeconds);
        if (Application.isPlaying && (cancellationTokenSource == null || !cancellationTokenSource.IsCancellationRequested)) // Double check if still needed
        {
            AttemptSseConnection();
        }
    }

    /// <summary>
    /// This is where you implement the logic to update your Unity scene
    /// based on the received detection data.
    /// </summary>
    /// <param name="detectionData">The deserialized detection data from the server.</param>
    void UpdateDetectionInfoInUnity(Dictionary<string, List<NewDetectedObjectInfo>> detectionData)
    {
        // Example: Log the received data. Replace with your actual game logic.
        // This logging can be very verbose if updates are frequent.
        // Debug.Log($"[SSEObjectReceiver] Processing {detectionData.Count} sectors in UpdateDetectionInfoInUnity.");

        foreach (var sectorEntry in detectionData)
        {
            Debug.Log(sectorEntry);
            string sectorKey = sectorEntry.Key; // e.g., "0-0", "1-2"
            List<NewDetectedObjectInfo> objectsInSector = sectorEntry.Value;

            if (objectsInSector.Count > 0)
            {
                // Example: Log details for sectors that have objects
                // Debug.Log($"Sector [{sectorKey}]: Found {objectsInSector.Count} object(s).");
                foreach (NewDetectedObjectInfo objInfo in objectsInSector)
                {
                    if (objInfo.sector_row == null) continue;

                    if (Farm.instance.plantsManager.plantList[objInfo.sector_row, objInfo.sector_col] == null)
                    {
                        Farm.instance.plantsManager.AddPlant(objInfo.sector_row, objInfo.sector_col);
                    }

                    PlantType type = PlantType.Cabbage;
                    if (objInfo.ObjectType == "cabbage") type = PlantType.Cabbage;
                    else if (objInfo.ObjectType == "tomato") type = PlantType.Tomato;
                    else if (objInfo.ObjectType == "eggplant") type = PlantType.Eggplant;

                    PlantLevel lv = PlantLevel.Lv1;
                    if (objInfo.Level == "v1") lv = PlantLevel.Lv1;
                    else if (objInfo.Level == "v2") lv = PlantLevel.Lv2;
                    else if (objInfo.Level == "v3") lv = PlantLevel.Lv3;
                    else if (objInfo.Level == "v4") lv = PlantLevel.Lv4;

                    Debug.Log(objInfo.ObjectType);
                    Debug.Log(objInfo.Level);
                    
                    Farm.instance.plantsManager.UpdatePlant(
                        Farm.instance.plantsManager.plantList[objInfo.sector_row, objInfo.sector_col],
                        objInfo.sector_row,
                        objInfo.sector_col,
                        type,
                        lv
                    );

                    // Access objInfo.sector_row, objInfo.sector_col, objInfo.Level, objInfo.ObjectType
                    // Example:
                    // Debug.Log($"  - Row: {objInfo.sector_row}, Col: {objInfo.sector_col}, Lv: {objInfo.Level}, Type: {objInfo.ObjectType}");
                    //
                    // YOUR UNITY UPDATE LOGIC GOES HERE:
                    // - Find GameObjects representing these sectors or objects.
                    // - Update UI elements (Text, Images, etc.).
                    // - Trigger game events.
                    // - Visualize bounding boxes (if you were to add coordinates back to server response).
                    //
                }
            }
            // else: This sector has no detected objects. You might want to clear previous visuals for this sector.
        }
    }

    void OnApplicationQuit()
    {
        Debug.Log("[SSEObjectReceiver] Application quitting. Cleaning up SSE connection.");
        if (cancellationTokenSource != null)
        {
            cancellationTokenSource.Cancel(); // Signal cancellation to the async task
            cancellationTokenSource.Dispose();
            cancellationTokenSource = null;
        }
        if (httpClient != null)
        {
            // HttpClient.CancelPendingRequests(); // Good practice, though Dispose should handle it
            httpClient.Dispose();
            httpClient = null;
        }
    }

    // It's also good practice to handle OnDestroy if the GameObject itself is destroyed
    // before the application quits.
    void OnDestroy()
    {
        Debug.Log("[SSEObjectReceiver] GameObject is being destroyed. Cleaning up SSE connection.");
        if (cancellationTokenSource != null && !cancellationTokenSource.IsCancellationRequested)
        {
            cancellationTokenSource.Cancel();
            // Dispose is handled in OnApplicationQuit, but if only the object is destroyed,
            // it might be good to dispose here too, or ensure OnApplicationQuit handles it.
            // For simplicity, OnApplicationQuit will handle final disposal.
        }
        // Stop any running coroutines like DelayedReconnect
        StopAllCoroutines();
    }
}