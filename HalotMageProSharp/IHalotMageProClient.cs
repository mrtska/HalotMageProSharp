using HalotMageProSharp.Events;

namespace HalotMageProSharp {
    /// <summary>
    /// A interface for the Halot Mage Pro WebSocket API client.
    /// </summary>
    public interface IHalotMageProClient {
        /// <summary>
        /// Connect to the Halot Mage Pro.
        /// </summary>
        void Connect();

        /// <summary>
        /// Triggered when incorrect password is provided.
        /// </summary>
        event EventHandler OnTokenError;

        /// <summary>
        /// Response to the version check request.
        /// </summary>
        event EventHandler<VersionCheckEvent> OnVersionCheck;
        
        /// <summary>
        /// Response to the printer status request.
        /// </summary>
        event EventHandler<GetPrinterStatusEvent> OnGetPrinterStatus;

        /// <summary>
        /// Response to the send file request.
        /// </summary>
        event EventHandler<SendFileEvent> OnSendFile;
        
        /// <summary>
        /// Response to the send file progress.
        /// </summary>
        event EventHandler<SendFileProgressEvent> OnSendFileProgress;
        
        /// <summary>
        /// Response to the send file request finalization.
        /// </summary>
        event EventHandler<OnCheckFileEvent> OnCheckFile;

        /// <summary>
        /// Response to the start print request.
        /// </summary>
        event EventHandler<OnStartPrintEvent> OnStartPrint;

        /// <summary>
        /// Response to the pause print request.
        /// </summary>
        event EventHandler<OnCommandEvent> OnPausePrint;

        /// <summary>
        /// Response to the stop print request.
        /// </summary>
        event EventHandler<OnCommandEvent> OnStopPrint;

        /// <summary>
        /// Response to the set print parameter request.
        /// </summary>
        event EventHandler<OnCommandEvent> OnSetPrintParameter;

        /// <summary>
        /// Previous printer status.
        /// </summary>
        GetPrinterStatusEvent? PreviousPrintStatus { get; }

        /// <summary>
        /// Latest printer status.
        /// </summary>
        GetPrinterStatusEvent? LatestPrintStatus { get; }

        /// <summary>
        /// Get the camera video stream Uri for the Halot Mage Pro.
        /// </summary>
        /// <returns>Rtsp protocol uri.</returns>
        Uri GetCameraVideoUri();

        /// <summary>
        /// Check the version of the Halot Mage Pro.
        /// </summary>
        void CheckVersion();

        /// <summary>
        /// Retrieve the current status of the printer.
        /// </summary>
        void GetPrinterStatus();

        /// <summary>
        /// Send a sliced file to the Halot Mage Pro.
        /// Must be a .cxdlpv4 file.
        /// </summary>
        /// <param name="filename">filename.</param>
        /// <param name="stream">Binary stream.</param>
        void SendFile(string filename, Stream stream);

        /// <summary>
        /// Start the print processing.
        /// </summary>
        /// <param name="filename">filename</param>
        void StartPrint(string filename);

        /// <summary>
        /// Stop the print processing.
        /// Platform will move to the home position.
        /// </summary>
        void StopPrint();

        /// <summary>
        /// Pause the print processing.
        /// </summary>
        void PausePrint();

        /// <summary>
        /// Resume the print processing.
        /// Same as PausePrint internally.
        /// </summary>
        void ResumePrint();

        /// <summary>
        /// Set the print parameters.
        /// This command only works when the printer is in the processing state.
        /// </summary>
        /// <param name="bottomExposureLayer">Layer count of initial layers.</param>
        /// <param name="lightOffDelay">Light off delay</param>
        /// <param name="motorSpeed">Moter Speed mm/s</param>
        /// <param name="initialExposure">Initial exposure time in second.</param>
        /// <param name="printExposure">Print exposure time in second.</param>
        /// <param name="risingHeight">Rising height in mm.</param>
        void SetPrintParameter(int bottomExposureLayer, int lightOffDelay, int motorSpeed, int initialExposure, double printExposure, int risingHeight);
    }
}
