using System;
using CommonModel.Kernel;
using CommonModel.RandomStreamProducing;
using System.Collections.Generic;
using System.IO;

namespace Model_Lab
{

    public partial class SmoModel : Model
    {
        //Условие завершения прогона модели True - завершить прогон. По умолчанию false. </summary>
        public override bool MustStopRun(int variantCount, int runCount)
        {
            return (isFinish);
        }

        //установка метода перебора вариантов модели
        public override bool MustPerformNextVariant(int variantCount)
        {
            //используем один вариант модели
            return variantCount < 1;
        }

        //true - продолжить выполнение ПРОГОНОВ модели;
        //false - прекратить выполнение ПРОГОНОВ модели. по умолчению false.
        public override bool MustPerformNextRun(int variantCount, int runCount)
        {
            return runCount < 1; //выполняем 1 прогон модели
        }

        //Задание начального состояния модели для нового варианта модели
        public override void SetNextVariant(int variantCount)
        {
            #region Параметры модели

            maxNCP = 10;
            NCP = 0;
            maxN = 10;
            currN = 0;
            maxNT = 10;
            minNT = 1;
            TTN = 0;
            measureNumber = 0;

            #endregion

            #region Установка параметров законов распределения

            #endregion
        }

        public override void StartModelling(int variantCount, int runCount)
        {
            ReadFile();
            //for (int i = 0; i < processes.Count; i++)
            //{
            //    VQ.Add(processes[i]);
            //}

            #region Задание начальных значений модельных переменных и объектов
            #endregion

            #region Cброс сборщиков статистики

            #endregion

            //Печать заголовка строки состояния модели
            TraceModelHeader();

            #region Планирование начальных событий      

            var ev = new FIFO();
            PlanEvent(ev, 0.0);

            #endregion
        }

        void ReadFile()
        {
            StreamReader sr;
            if (Environment.OSVersion.Platform.ToString() == "Win32NT")
            {
                sr = new StreamReader(@"D:\Langs\C#\SPOlab1\input.txt");

            }
            else

            {
                sr = new StreamReader(@"/Users/andreymakarov/Downloads/SPOlab1/input.txt");
            }

            Tracer.AnyTrace(Environment.OSVersion.Platform.ToString());
            while (!sr.EndOfStream)
            {
                String[] tmp = sr.ReadLine().Split();
                Process process = new Process(Convert.ToInt32(tmp[0]),
                                              Convert.ToInt32(tmp[1]),
                                              Convert.ToInt32(tmp[2]),
                                              0,
                                              Convert.ToInt32(tmp[3]));
                Tracer.AnyTrace("Процесс №" + process.number + 
                                " с временем разблокировки " + process.readinessTime + 
                                " временем выполнения " + process.requiredAmount + 
                                " приоритетом " + process.priority);

                processes.Add(process);
            }

            foreach (Process process in processes)
            {
                waitProcesses.Add(new Process(process.number, 
                                              process.readinessTime, 
                                              process.requiredAmount, 
                                              0,
                                              process.priority));
            }

            //waitProcesses[0].requiredAmount--;

            Tracer.AnyTrace("==================================================");
            foreach (Process process in processes)
            {
                Tracer.AnyTrace("Процесс №" + (process.number) + " добавлен в очередь" +
  " " + (process.readinessTime) + " " + (process.requiredAmount));
            }
            Tracer.AnyTrace("===================================================");
            Tracer.AnyTrace(waitProcesses.Count + " " + processes.Count);
        }

        //Действия по окончанию прогона
        public override void FinishModelling(int variantCount, int runCount)
        {
            Tracer.AnyTrace("");
            Tracer.TraceOut("==============================================================");
            Tracer.TraceOut("============Статистические результаты моделирования===========");
            Tracer.TraceOut("==============================================================");
            Tracer.AnyTrace("");

            Tracer.AnyTrace("Суммарное количество затраченных тактов процессора для FIFO: " + (fifoMeasureNumber-1));
            Tracer.AnyTrace("Суммарное количество затраченных тактов процессора для SJF: " + (measureNumber-1));
            //Tracer.TraceOut("Время моделирования: " + String.Format("{0:0.00}", Time));

        }

        //Печать заголовка
        void TraceModelHeader()
        {
            Tracer.TraceOut("==============================================================");
            Tracer.TraceOut("======================= Запущена модель ======================");
            Tracer.TraceOut("==============================================================");
            //вывод заголовка трассировки
            Tracer.AnyTrace("");
            Tracer.AnyTrace("Параметры модели:");
            Tracer.AnyTrace("");
        }

        //Печать строки состояния
        void TraceModel()
        {
        }

    }
}

