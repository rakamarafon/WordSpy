namespace WordSpy.DTO_s
{
    public class StartParamsDTO
    {
        public string URL { get; set; }
        public int MaxThreads { get; set; }
        public string TextToFind { get; set; }
        public int MaxScanURLs { get; set; }
    }
}
