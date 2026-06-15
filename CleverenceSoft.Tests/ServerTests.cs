using CleverenceSoftTasks;
using System.Reflection;

namespace CleverenceSoftTestLib
{
    [TestFixture]
    public class ServerTests
    {
        private const int ThreadCount = 10;
        private const int IterationsPerThread = 1000;

        [Test]
        public void AddToCount_WhenCalledConcurrently_ShouldBeConsistent()
        {
            var field = typeof(Server).GetField("_count", BindingFlags.Static | BindingFlags.NonPublic);
            field.SetValue(null, 0);

            var threads = new Thread[ThreadCount];
            for (int i = 0; i < ThreadCount; i++)
            {
                threads[i] = new Thread(() =>
                {
                    for (int j = 0; j < IterationsPerThread; j++)
                    {
                        Server.AddToCount(1);
                    }
                });
                threads[i].Start();
            }

            foreach (var t in threads)
                t.Join();

            int expected = ThreadCount * IterationsPerThread;
            Assert.That(Server.GetCount(), Is.EqualTo(expected));
        }

        [Test]
        public void GetCount_WhenReadDuringWrite_ShouldWait()
        {
            var lockField = typeof(Server).GetField("_lock", BindingFlags.Static | BindingFlags.NonPublic);
            var countField = typeof(Server).GetField("_count", BindingFlags.Static | BindingFlags.NonPublic);
            var rwLock = (ReaderWriterLockSlim)lockField.GetValue(null);

            using var writeLockAcquired = new ManualResetEventSlim(false);
            using var readAttempted = new ManualResetEventSlim(false);
            using var writeDone = new ManualResetEventSlim(false);

            int? readResult = null;

            var writer = new Thread(() =>
            {
                rwLock.EnterWriteLock();
                try
                {
                    writeLockAcquired.Set();

                    readAttempted.Wait();

                    Thread.Sleep(200);

                    countField.SetValue(null, 42);
                }
                finally
                {
                    rwLock.ExitWriteLock();
                    writeDone.Set();
                }
            });

            var reader = new Thread(() =>
            {
                writeLockAcquired.Wait();
                readAttempted.Set();

                readResult = Server.GetCount();
            });

            writer.Start();
            reader.Start();

            Thread.Sleep(50);

            Assert.IsNull(readResult, "Читатель не должен был получить значение, пока писатель не завершил запись");

            writeDone.Wait();

            reader.Join(500);
            Assert.IsNotNull(readResult);
            Assert.AreEqual(42, readResult, "Читатель должен получить значение, изменённое писателем");

            writer.Join();
        }
    }
}
