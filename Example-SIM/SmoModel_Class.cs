using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CommonModel.StatisticsCollecting;
using CommonModel.RandomStreamProducing;
using CommonModel.Collections;
using CommonModel.Kernel;

namespace Model_Lab
{

    public partial class SmoModel : Model
    {

        #region Параметры модель

        /* Number of completed processes */
        int NCP;
        int fifoTickNumber = 0;
        /* Maximum number of completed processes */
        int maxNCP;
        /* List of processes */
        List<Process> processes = new List<Process>();
        /* List of processes */
        List<Process> waitProcesses = new List<Process>();

        List<Process> allProcesses = new List<Process>();

        List<Process> allSjfProcesses = new List<Process>();

        List<String> processesStates = new List<String>();
        /* Tick number */
        int tickNumber;

        List<String> fifoTrace = new List<String>();
        List<String> sjfTrace = new List<String>();

        int fifoWaitTime = 0;
        int fifoExecTime = 0;

        int sjfWaitTime = 0;
        int sjfExecTime = 0;

        bool isFinish = false;
        bool newProcess = false;

        #endregion

        #region Дополнительные структуры

        /* Process */
        public class Process : QueueRecord
        {
            /* Number of process*/
            public int number;
            /* Required number of ticks for execution */
            public int requiredAmount;
            /* Priority */
            public int priority;
            /* Readiness time */
            public int readinessTime;

            public Process(int _number, int _readinessTime, int _requiredAmount, int _priority)
            {
                number = _number;
                requiredAmount = _requiredAmount;
                priority = _priority;
                readinessTime = _readinessTime;
            }
        }

        /* Queue for FIFO */
        SimpleModelList<Process> QFIFO;

        #endregion

        #region Cборщики статистики

        #endregion

        #region Инициализация объектов модели

        public SmoModel(Model parent, string name) : base(parent, name)
        {
            QFIFO = InitModelObject<SimpleModelList<Process>>();
        }

        #endregion
    }
}
