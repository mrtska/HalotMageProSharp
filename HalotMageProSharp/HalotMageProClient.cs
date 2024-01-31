using HalotMageProSharp.Events;
using System.Net;
using System.Net.WebSockets;
using System.Text.Json;
using System.Text.Json.Nodes;
using Websocket.Client;

namespace HalotMageProSharp {
    /// <summary>
    /// An implementation for the Halot Mage Pro WebSocket API client.
    /// </summary>
    public class HalotMageProClient : IHalotMageProClient, IDisposable {

        public IPAddress IPAddress { get; private set; }
        public string Password { get; set; }

        private readonly WebsocketClient Client;

        public HalotMageProClient(IPAddress address, string password) {
            IPAddress = address;
            Password = password;

            Client = new WebsocketClient(new Uri($"ws://{IPAddress}:18188"), () => {
                return new ClientWebSocket() {
                    Options = {
                        KeepAliveInterval = TimeSpan.Zero
                    }
                };
            }) {
                ReconnectTimeout = TimeSpan.FromMinutes(10)
            };

            Client.ReconnectionHappened.Subscribe(info => {
                OnConnected?.Invoke(this, EventArgs.Empty);
            });
            Client.DisconnectionHappened.Subscribe(info => {
                OnDisconnected?.Invoke(this, EventArgs.Empty);
            });

            // Handle websocket messages.
            Client.MessageReceived.Subscribe(msg => {
                // Ignore non-text messages.
                if (msg.MessageType != WebSocketMessageType.Text) {
                    return;
                }
                var json = JsonNode.Parse(msg.Text!)!;

                if (json["version"]?.ToString() is string version) {
                    OnVersionCheck?.Invoke(this, new VersionCheckEvent {
                        Version = version
                    });
                    return;
                }

                var cmd = json["cmd"]?.ToString();

                if (cmd == "GET_PRINT_STATUS") {
                    static int? convert(string? value) {
                        return string.IsNullOrEmpty(value) ? null : int.Parse(value);
                    }
                    static double? convertDouble(string? value) {
                        return string.IsNullOrEmpty(value) ? null : double.Parse(value);
                    }

                    PreviousPrintStatus = LatestPrintStatus;
                    LatestPrintStatus = new GetPrinterStatusEvent {
                        PrintStatus = json["printStatus"]?.ToString() ?? "UNKNOWN_ERROR",
                        Filename = json["filename"]?.ToString(),
                        SliceLayerCount = convert(json["sliceLayerCount"]?.ToString()),
                        CurrentSliceLayer = convert(json["curSliceLayer"]?.ToString()),
                        PrintRemainTime = convert(json["printRemainTime"]?.ToString()),
                        InitialExposure = convert(json["initExposure"]?.ToString()),
                        LightOffDelay = convert(json["delayLight"]?.ToString()),
                        PrintExposure = convertDouble(json["printExposure"]?.ToString()),
                        RisingHeight = convert(json["printHeight"]?.ToString()),
                        MotorSpeed = convert(json["eleSpeed"]?.ToString()),
                        BottomExposureLayerNum = convert(json["bottomExposureNum"]?.ToString()),
                        LayerThickness = convertDouble(json["layerThickness"]?.ToString()),
                        Resin = json["resin"]?.ToString()
                    };

                    if (LatestPrintStatus.PrintStatus == "TOKEN_ERROR") {
                        OnTokenError?.Invoke(this, EventArgs.Empty);
                    } else {
                        OnGetPrinterStatus?.Invoke(this, LatestPrintStatus);
                    }
                    return;
                }

                if (cmd == "START_PRINT") {
                    var status = json["status"]?.ToString();
                    var filename = json["filename"]?.ToString();

                    OnStartPrint?.Invoke(this, new OnStartPrintEvent {
                        Status = status!,
                        Filename = filename!
                    });
                    return;
                }

                if (cmd == "START_FILE") {
                    var filename = json["filename"]?.ToString();
                    var key = json["key"]?.ToString();
                    var offset = json["offset"]?.ToString();
                    var size = json["size"]?.ToString();
                    var compress = (bool)json["compress"]!;
                    var errorCode = (int)json["errorcode"]!;

                    OnSendFile?.Invoke(this, new SendFileEvent {
                        Filename = filename!,
                        Key = key!,
                        Offset = int.Parse(offset!),
                        Size = int.Parse(size!),
                        Compress = compress,
                        ErrorCode = errorCode
                    });
                    return;
                }

                if (cmd == "START_DATA") {
                    var key = json["key"]?.ToString();
                    var received = json["received"]?.ToString();
                    var size = json["size"]?.ToString();
                    var errorCode = (int)json["errorcode"]!;

                    OnSendFileProgress?.Invoke(this, new SendFileProgressEvent {
                        Key = key!,
                        Received = int.Parse(received!),
                        Size = int.Parse(size!),
                        ErrorCode = errorCode
                    });
                    return;
                }

                if (cmd == "CHECK_DATA") {
                    var key = json["key"]?.ToString();
                    var checkState = (int)json["checkstate"]!;

                    OnCheckFile?.Invoke(this, new OnCheckFileEvent {
                        Key = key!,
                        CheckState = checkState
                    });
                    return;
                }

                if (cmd == "PRINT_STOP") {
                    var status = json["status"]?.ToString();

                    OnStopPrint?.Invoke(this, new OnCommandEvent {
                        Status = status!
                    });
                    return;
                }

                if (cmd == "PRINT_PAUSE") {
                    var status = json["status"]?.ToString();

                    OnPausePrint?.Invoke(this, new OnCommandEvent {
                        Status = status!
                    });
                    return;
                }

                if (cmd == "PRINT_PARA_SET") {
                    var status = json["status"]?.ToString();

                    OnSetPrintParameter?.Invoke(this, new OnCommandEvent {
                        Status = status!
                    });
                    return;
                }
            });
        }

        public GetPrinterStatusEvent? PreviousPrintStatus { get; private set; }
        public GetPrinterStatusEvent? LatestPrintStatus { get; private set; }

        public event EventHandler? OnConnected;
        public event EventHandler? OnDisconnected;
        public event EventHandler? OnTokenError;
        public event EventHandler<VersionCheckEvent>? OnVersionCheck;
        public event EventHandler<GetPrinterStatusEvent>? OnGetPrinterStatus;
        public event EventHandler<SendFileEvent>? OnSendFile;
        public event EventHandler<SendFileProgressEvent>? OnSendFileProgress;
        public event EventHandler<OnCheckFileEvent>? OnCheckFile;
        public event EventHandler<OnStartPrintEvent>? OnStartPrint;
        public event EventHandler<OnCommandEvent>? OnPausePrint;
        public event EventHandler<OnCommandEvent>? OnStopPrint;
        public event EventHandler<OnCommandEvent>? OnSetPrintParameter;

        /// <inheritdoc />
        public void Connect() {
            Client.Start();
        }

        public void Dispose() {
            Client.Dispose();
        }

        /// <inheritdoc />
        public Uri GetCameraVideoUri() {
            return new Uri($"rtsp://{IPAddress}/ch0_0");
        }

        /// <inheritdoc />
        public void GetPrinterStatus() {
            Client.Send(JsonSerializer.Serialize(new {
                cmd = "GET_PRINT_STATUS",
                token = CryptoUtil.GetToken(Password)
            }));
        }

        /// <inheritdoc />
        public void PausePrint() {
            Client.Send(JsonSerializer.Serialize(new {
                cmd = "PRINT_PAUSE",
                token = CryptoUtil.GetToken(Password)
            }));
        }

        /// <inheritdoc />
        public void ResumePrint() {
            // Same as pause.
            PausePrint();
        }

        /// <inheritdoc />
        public void SendFile(string filename, Stream stream) {

            if (string.IsNullOrEmpty(filename)) {
                throw new ArgumentNullException(nameof(filename));
            }
            if (stream is null) {
                throw new ArgumentNullException(nameof(stream));
            }

            Client.Send(JsonSerializer.Serialize(new {
                cmd = "START_FILE",
                filename,
                key = stream.GetHashCode().ToString("X").ToLowerInvariant(),
                offset = "0",
                size = stream.Length.ToString(),
            }));

            var memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);
            memoryStream.Position = 0;

            Client.Send(memoryStream.ToArray());
        }

        /// <inheritdoc />
        public void SetPrintParameter(int bottomExposureLayer, int lightOffDelay, int motorSpeed, int initialExposure, double printExposure, int risingHeight) {

            if (bottomExposureLayer is not >= 1 and <= 10) {
                throw new ArgumentOutOfRangeException(nameof(bottomExposureLayer));
            }
            if (lightOffDelay is not >= 0 and <= 20) {
                throw new ArgumentOutOfRangeException(nameof(lightOffDelay));
            }
            if (motorSpeed is not >= 1 and <= 10) {
                throw new ArgumentOutOfRangeException(nameof(motorSpeed));
            }
            if (initialExposure is not >= 1 and <= 70) {
                throw new ArgumentOutOfRangeException(nameof(initialExposure));
            }
            if (printExposure is not >= 0.5 and <= 10.0) {
                throw new ArgumentOutOfRangeException(nameof(printExposure));
            }
            if (risingHeight is not >= 8 and <= 20) {
                throw new ArgumentOutOfRangeException(nameof(risingHeight));
            }

            Client.Send(JsonSerializer.Serialize(new {
                cmd = "PRINT_PARA_SET",
                bottomExposureNum = bottomExposureLayer.ToString(),
                delayLight = lightOffDelay.ToString(),
                eleSpeed = motorSpeed.ToString(),
                initExposure = initialExposure.ToString(),
                printExposure = printExposure.ToString(),
                printHeight = risingHeight.ToString(),
                token = CryptoUtil.GetToken(Password)
            }));
        }

        /// <inheritdoc />
        public void StartPrint(string filename) {
            Client.Send(JsonSerializer.Serialize(new {
                cmd = "START_PRINT",
                filename,
                token = CryptoUtil.GetToken(Password)
            }));
        }

        /// <inheritdoc />
        public void StopPrint() {
            Client.Send(JsonSerializer.Serialize(new {
                cmd = "PRINT_STOP",
                token = CryptoUtil.GetToken(Password)
            }));
        }

        /// <inheritdoc />
        public void CheckVersion() {
            Client.Send(JsonSerializer.Serialize(new {
                cmd = "VERSION_CHECK",
            }));
        }
    }
}
