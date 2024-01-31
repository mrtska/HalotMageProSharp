namespace HalotMageProSharp.Events {
    public record VersionCheckEvent {
        /// <summary>
        /// "1" will return.
        /// </summary>
        public string Version { get; init; } = default!;
    }
}
