using CsvHelper;
using CsvHelper.Configuration;
using Meta.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loggers.CSV
{
    public class CSVLogger<T, TRecord> : ILogger<TRecord>, IDisposable where T : ISpecimen<T> where TRecord : IRecord
    {
        public string FilePath { get; set; }
        public Task LoggerThread => this.loggerThread;
        public Queue<Task> LoggerTasks { get; }
        public object loggerLock;

        private FileStream CsvStream { get; set; }
        private CsvWriter CsvWriter { get; set; }
        private Task loggerThread;
        public CancellationTokenSource CancellationTokenSource { get; }

        public CSVLogger(string filePath)
        {
            this.FilePath = filePath;
            this.CancellationTokenSource = new CancellationTokenSource();
            this.LoggerTasks = new Queue<Task>(500);
            this.loggerLock = new object();
        }

        public void RunLogger()
        {
            var directoryPath = Path.GetDirectoryName(this.FilePath);
            if (!string.IsNullOrWhiteSpace(directoryPath))
            {
                var directoryInfo = new DirectoryInfo(directoryPath);
                if (!directoryInfo.Exists)
                {
                    directoryInfo.Create();
                }
            }
            this.CsvStream = new FileStream(this.FilePath, FileMode.Create, FileAccess.Write, FileShare.Read);
            this.CsvWriter = new CsvWriter(new StreamWriter(this.CsvStream)
                , new CsvConfiguration(CultureInfo.CurrentCulture)
                {
                    Delimiter = ";",
                }
            );
            if (!this.CsvStream.CanWrite)
            {
                throw new IOException($"CSVLogger: Cannot create file for logging at {this.FilePath}");
            }
            this.CsvWriter.WriteHeader(typeof(TRecord));
            this.CsvWriter.NextRecord();
            this.loggerThread = new Task(LoggerLoop);
            this.loggerThread.Start();
        }

        private void LoggerLoop()
        {
            while (!this.CancellationTokenSource.Token.IsCancellationRequested)
            {
                if(this.LoggerTasks.Count == 0)
                {
                    Thread.Sleep(100);
                }
                else
                {
                    var task = this.LoggerTasks.Dequeue();
                    if(task != null)
                    {
                        task.Start();
                        task.Wait();
                    }
                }
            }
        }

        public Task Log(TRecord record)
        {
            TRecord? recordCopy = record;
            Task newTask = new Task(() =>
            {
                LogSpecimens(recordCopy);
            });
            lock (loggerLock)
            {
                this.LoggerTasks.Enqueue(newTask);
            }
            return newTask;
        }

        private void LogSpecimens(TRecord record)
        {
            this.CsvWriter.WriteRecord(record);
            this.CsvWriter.NextRecord();
            this.CsvWriter.Flush();
        }

        public void Wait()
        {
            while(this.LoggerTasks.Count > 0)
            {
                Thread.Sleep(50);
            }
        }

        public void Dispose()
        {
            if (!this.CancellationTokenSource.IsCancellationRequested)
            {
                this.CancellationTokenSource.Cancel();
            }
            this.LoggerThread.Wait();
            this.CsvWriter.Dispose();
        }
    }
}
