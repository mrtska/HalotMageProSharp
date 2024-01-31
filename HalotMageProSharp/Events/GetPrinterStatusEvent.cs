namespace HalotMageProSharp.Events {
    public record GetPrinterStatusEvent {
        /// <summary>
        /// The current status of the printer.
        /// 
        /// PRINT_GENERAL, PRINT_PROCESSING, PRINT_STOPPING, PRINT_STOP, PRINT_COMPLETING, PRINT_COMPLETE
        /// </summary>
        public string PrintStatus { get; init; } = default!;

        /// <summary>
        /// Filename of the current print job.
        /// </summary>
        public string? Filename { get; init; }

        /// <summary>
        /// The total layer count of the print job.
        /// </summary>
        public int? SliceLayerCount { get; init; }

        /// <summary>
        /// The current progress of the print job.
        /// </summary>
        public int? CurrentSliceLayer { get; init; }

        /// <summary>
        /// The current progress of the print job in second.
        /// </summary>
        public int? PrintRemainTime { get; init; }

        /// <summary>
        /// Initial exposure time of the print job.
        /// </summary>
        public int? InitialExposure { get; init; }

        /// <summary>
        /// Light off delay of the print job.
        /// </summary>
        public int? LightOffDelay { get; init; }

        /// <summary>
        /// Print exposure time of the print job.
        /// </summary>
        public double? PrintExposure { get; init; }

        /// <summary>
        /// Rising height of the print job.
        /// </summary>
        public int? RisingHeight { get; init; }

        /// <summary>
        ///  Motor speed of the print job.
        /// </summary>
        public int? MotorSpeed { get; init; }

        /// <summary>
        ///  Bottom exposure layer count of the print job.
        /// </summary>
        public int? BottomExposureLayerNum { get; init; }

        /// <summary>
        ///  Current layer thickness of the print job.
        /// </summary>
        public double? LayerThickness { get; init; }

        /// <summary>
        /// Unknown.
        /// </summary>
        public string? Resin { get; init; }
    }
}
