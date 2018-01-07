﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LiteDB
{
    /// <summary>
    /// A class to control locking disposal states
    /// </summary>
    public class LockReadWrite : IDisposable
    {
        public ReaderWriterLockSlim Reader { get; private set; }
        public ReaderWriterLockSlim Reserved { get; set; } = null;
        public List<Tuple<string, ReaderWriterLockSlim>> Collections { get; set; } = new List<Tuple<string, ReaderWriterLockSlim>>();

        private Logger _log;

        internal LockReadWrite(ReaderWriterLockSlim reader, Logger log)
        {
            this.Reader = reader;
            _log = log;
        }

        /// <summary>
        /// When dipose, exit all lock states
        /// </summary>
        public void Dispose()
        {
            _log.LockExit(this.Reader, this.Reserved, this.Collections);

            this.Reader.ExitReadLock();

            if (this.Reserved != null)
            {
                this.Reserved.ExitWriteLock();
            }

            foreach(var col in this.Collections)
            {
                col.Item2.ExitWriteLock();
            }
        }
    }
}