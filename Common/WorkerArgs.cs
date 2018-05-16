namespace Common
{
    public class WorkerArgs
    {
        public string InputFileName { get; set; }
        public string OutputFileName { get; set; }
        public int MaxThreads { get; set; }
        public bool Verbose { get; set; }
    }
}