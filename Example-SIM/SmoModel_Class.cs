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

        /* Maximum number of processes */
        int maxN;
        /* Maximum number of measures for one process execution */
        int maxNT;
        /* Minimal number of measures for one process execution */
        int minNT;
        /* Number of completed processes */
        int NCP;
        /* Maximum number of completed processes */
        int maxNCP;
        /* Total number of measures */
        int TTN;
        /* List of processes */
        List<Process> processes = new List<Process>();
        /* List of processes */
        List<Process> waitProcesses = new List<Process>();
        /* Measure number */
        int measureNumber;

        #endregion

        #region Переменные состояния модели

        /* Current number of processes */
        int currN;

        #endregion

        #region Дополнительные структуры

        /* Process */
        public class Process : QueueRecord
        {
            /* Number of process*/
            public int number;
            /* Required number of measures for execution */
            public int requiredAmount;
            /* Current number of measures for execution */
            public int currentAmount;
            /* Priority */
            public int priority;
            /* Readiness time */
            public int readinessTime;

            public Process(int _number, int _readinessTime, int _requiredAmount, int _currentAmount, int _priority)
            {
                number = _number;
                requiredAmount = _requiredAmount;
                currentAmount = _currentAmount;
                priority = _priority;
                readinessTime = _readinessTime;
            }
        }

        //// Элемент очереди заявки в узлах ВС 
        //class QRec : QueueRecord
        //{
        //    public Process Z;

        //    public QRec(Process _Z)
        //    {
        //        Z = _Z;
        //    }
        //}

        // Группа очередей ПП
        SimpleModelList<Process> VQ;
        // Группа очередей ПП
        //SimpleModelList<Process> VQO;

        #endregion

        #region Cборщики статистики

        #endregion

        #region Генераторы ПСЧ

        #endregion

        #region Инициализация объектов модели

        public SmoModel(Model parent, string name) : base(parent, name)
        {
            VQ = InitModelObject<SimpleModelList<Process>>();
            //VQO = InitModelObject<SimpleModelList<Process>>();
        }

        #endregion
    }
}
