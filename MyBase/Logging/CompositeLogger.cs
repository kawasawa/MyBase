using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace MyBase.Logging
{
    /// <summary>
    /// 複数のロガーを束ねるコンテナを表します。
    /// </summary>
    public class CompositeLogger : ICollection<ILoggerFacade>, ILoggerFacade
    {
        private const int SHRINK_THRESHOLD = 64;
        private const int DEFAULT_CAPACITY = 16;

        private readonly object _lockToken = new object();
        private List<ILoggerFacade> _loggers;
        private int _count;

        /// <summary>
        /// 格納されている要素の数を取得します。
        /// </summary>
        public int Count => Volatile.Read(ref this._count);

        /// <summary>
        /// 読み取り専用かどうかを示す値を取得します。
        /// </summary>
        public bool IsReadOnly => false;

        /// <summary>
        /// このクラスの新しいインスタンスを生成します。
        /// </summary>
        public CompositeLogger()
        {
            this._loggers = new List<ILoggerFacade>();
        }

        /// <summary>
        /// このクラスの新しいインスタンスを生成します。
        /// </summary>
        /// <param name="capacity">初期容量</param>
        public CompositeLogger(int capacity)
        {
            if (capacity < 0)
                throw new ArgumentOutOfRangeException(nameof(capacity));

            this._loggers = new List<ILoggerFacade>(capacity);
        }

        /// <summary>
        /// このクラスの新しいインスタンスを生成します。
        /// </summary>
        /// <param name="loggers">初期要素</param>
        public CompositeLogger(params ILoggerFacade[] loggers)
        {
            if (loggers == null)
                throw new ArgumentNullException(nameof(loggers));

            this.InternalInitialize(loggers, loggers.Length);
        }

        /// <summary>
        /// このクラスの新しいインスタンスを生成します。
        /// </summary>
        /// <param name="loggers">初期要素</param>
        public CompositeLogger(IEnumerable<ILoggerFacade> loggers)
        {
            if (loggers == null)
                throw new ArgumentNullException(nameof(loggers));

            this.InternalInitialize(loggers, loggers is ICollection<ILoggerFacade> c ? c.Count : DEFAULT_CAPACITY);
        }

        /// <summary>
        /// 内包するコレクションを初期化します。
        /// </summary>
        /// <param name="loggers">ロガー</param>
        /// <param name="capacity">初期容量</param>
        private void InternalInitialize(IEnumerable<ILoggerFacade> loggers, int capacity)
        {
            this._loggers = new List<ILoggerFacade>(capacity);
            this._loggers.AddRange(loggers);
            Volatile.Write(ref this._count, this._loggers.Count);
        }

        #region ILoggerFacade

        /// <summary>
        /// ログを出力します。
        /// </summary>
        /// <param name="message">メッセージ</param>
        /// <param name="category">ログの種類</param>
        /// <param name="priority">ログの優先度</param>
        public void Log(string message, Category category, Priority priority)
            => this._loggers.ForEach(l => l?.Log(message, category, priority));

        #endregion

        #region ICollection

        /// <summary>
        /// 指定された項目を追加します。
        /// </summary>
        /// <param name="item">追加する項目</param>
        public void Add(ILoggerFacade item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            lock (this._lockToken)
            {
                this._loggers.Add(item);
                Volatile.Write(ref this._count, this._count + 1);
            }
        }

        /// <summary>
        /// 指定された項目が格納されている場合、最初に出現したものを削除します。
        /// </summary>
        /// <param name="item">削除する項目</param>
        /// <returns>項目が削除されたかどうかを示す値</returns>
        public bool Remove(ILoggerFacade item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            lock (this._lockToken)
            {
                var i = this._loggers.IndexOf(item);
                if (i < 0)
                    return false;

                this._loggers[i] = null;
                if (SHRINK_THRESHOLD < this._loggers.Capacity && this.Count < this._loggers.Capacity / 2)
                {
                    var remp = new List<ILoggerFacade>(this._loggers.Capacity / 2);
                    remp.AddRange(this._loggers);
                    this._loggers = remp;
                }
                Volatile.Write(ref this._count, this._count - 1);
                return true;
            }
        }

        /// <summary>
        /// すべての項目を削除します。
        /// </summary>
        public void Clear()
        {
            lock (this._lockToken)
            {
                this._loggers.Clear();
                Volatile.Write(ref this._count, 0);
            }
        }

        /// <summary>
        /// 指定された項目が格納されているかどうかを確認します。
        /// </summary>
        /// <param name="item">確認する項目</param>
        /// <returns>格納されているかどうかを示す値</returns>
        public bool Contains(ILoggerFacade item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            lock (this._lockToken)
            {
                return this._loggers.Contains(item);
            }
        }

        /// <summary>
        /// 格納されている要素を指定された配列にコピーします。
        /// </summary>
        /// <param name="array">コピー先の配列</param>
        /// <param name="arrayIndex">コピーを開始するインデックス</param>
        public void CopyTo(ILoggerFacade[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));
            if (arrayIndex < 0 || array.Length <= arrayIndex)
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));

            lock (this._lockToken)
            {
                if (arrayIndex + this._count > array.Length)
                    throw new ArgumentOutOfRangeException(nameof(arrayIndex));

                Array.Copy(this._loggers.ToArray(), 0, array, arrayIndex, this._loggers.Count);
            }
        }

        /// <summary>
        /// コレクションを繰り返し処理する列挙子を取得します。
        /// </summary>
        /// <returns>列挙子</returns>
        public IEnumerator<ILoggerFacade> GetEnumerator()
        {
            var result = default(IEnumerable<ILoggerFacade>);
            lock (this._lockToken)
            {
                result = this._loggers.ToList();
            }
            return result.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
            => this.GetEnumerator();

        #endregion
    }
}
