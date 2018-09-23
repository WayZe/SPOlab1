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

        #endregion

        #region Переменные состояния модели

        /* Current number of processes */
        int currN;

        #endregion

        #region Дополнительные структуры

        /* Process */
        public class Process
        {
            /* Number of process*/
            public int NP;
            /* Required number of measures for execution */
            public int RNT;
            // Требуемое количество квантов
            public int CNT;

            public Process(int _NP, int _RNT, int _CNT)
            {
                NP = _NP;
                RNT = _RNT;
                CNT = _CNT;
            }
        }

        // Элемент очереди заявки в узлах ВС 
        class QRec : QueueRecord
        {
            public Process Z;

            public QRec(Process _Z)
            {
                Z = _Z;
            }
        }

        // Группа очередей ПП
        SimpleModelList<QRec> VQ;

        #endregion

        #region Cборщики статистики

        #endregion

        #region Генераторы ПСЧ
        
        #endregion

        #region Инициализация объектов модели

        public SmoModel(Model parent, string name) : base(parent, name)
        {
            VQ = InitModelObject<SimpleModelList<QRec>>();
        }

        #endregion
    }
}
