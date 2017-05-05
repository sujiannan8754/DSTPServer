using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace DSTP_BLL.ClassToRtu
{
    /// <summary>
    /// 本类用于实现一个线程安全的事件队列，可以清队列，获取队员数量，入队，出队等操作。
    /// </summary>
    public class ClassSafeEventQueue
    {
        public Mutex mutexAccessThisQueue;
        private Queue myQueue;
        private int maxItemNumber; //最大成员数量
        public ClassSafeEventQueue()
        {
            myQueue = new Queue();
            myQueue.Clear();
            mutexAccessThisQueue = new Mutex();
            maxItemNumber = 10;
        }
        public ClassSafeEventQueue(int maxNumber)
        {
            myQueue = new Queue();
            myQueue.Clear();
            mutexAccessThisQueue = new Mutex();
            maxItemNumber = maxNumber;
        }
        /// <summary>
        /// 获取队列中的元素数量
        /// </summary>
        /// <returns></returns>
        public int getQitemCount()
        {
            int count = 0;
            mutexAccessThisQueue.WaitOne();
            try
            {
                count = myQueue.Count;
            }
            catch (System.Exception ex)
            {
                count = 0;
            }
            mutexAccessThisQueue.ReleaseMutex();
            return count;

        }


        /// <summary>
        /// 清除队列
        /// </summary>
        /// <returns></returns>
        public bool clearQ()
        {
            bool bRst = false;
            mutexAccessThisQueue.WaitOne();
            try
            {

                if (myQueue != null)
                {

                    myQueue.Clear();

                    bRst = true;
                }
            }
            catch (System.Exception ex)
            {
                bRst = false;
            }
            mutexAccessThisQueue.ReleaseMutex();
            return bRst;
        }


        /// <summary>
        ///  事件入队
        /// </summary>
        /// <param name="myEvent"></param>
        /// <returns></returns>
        public bool PutToQ(Object myEvent)
        {
            bool bRst = false;
            int count = 0;
            mutexAccessThisQueue.WaitOne();
            try
            {
                if (myQueue != null)
                {

                    try
                    {
                        count = myQueue.Count;
                    }
                    catch (System.Exception ex)
                    {
                        count = maxItemNumber;
                    }
                    if (count < maxItemNumber)
                    {
                        myQueue.Enqueue(myEvent);
                        bRst = true;
                    }

                }

            }
            catch (System.Exception ex)
            {
                bRst = false;
            }
            mutexAccessThisQueue.ReleaseMutex();
            return bRst;
        }

        /// <summary>
        /// 事件出队
        /// </summary>
        /// <param name="myEvent"></param>
        /// <returns></returns>
        public bool GetFromQ(ref Object myEvent)
        {
            bool bRst = false;
            mutexAccessThisQueue.WaitOne();
            try
            {
                if (myQueue.Count > 0)
                {
                    myEvent = (Object)myQueue.Dequeue();

                    bRst = true;
                }
                else
                {

                    bRst = false;
                }


            }
            catch (System.Exception ex)
            {
                bRst = false;
            }
            mutexAccessThisQueue.ReleaseMutex();
            return bRst;
        }

    }
}
