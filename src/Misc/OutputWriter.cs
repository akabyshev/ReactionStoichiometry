namespace ReactionStoichiometry
{
    internal class OutputWriter
    {
        private readonly string _path;
        private readonly List<string> _strings = new();

        internal OutputWriter(string path)
        {
            _path = path;
        }

        internal void Save()
        {
            File.WriteAllText(_path, string.Join(Environment.NewLine, _strings));
        }

        internal void WritePlainText(IBalancer balancer)
        {
            WriteLine(balancer.SimpleStackedOutput());
        }

        internal void WriteLine(string line)
        {
            _strings.Add(line);
        }
    }
}
