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
            tickNumber = 0;

            #endregion

            #region Установка параметров законов распределения

            //(generator.BPN as GeneratedBaseRandomStream).Seed = 1;
            //generator.A = 0;
            //generator.B = 2;

            #endregion
        }

        public override void StartModelling(int variantCount, int runCount)
        {
            GenerateFile1();

            ReadFile();

            GenerateFile2();

            for (int i = 0; i < processes.Count; i++)
            {
                processesStates.Add("-");
            }

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

        void GenerateFile1()
        {
            StreamWriter sw;
            if (Environment.OSVersion.Platform.ToString() == "Win32NT")
            {
                sw = new StreamWriter(@"D:\Langs\C#\SPOlab1\input.txt");

            }
            else
            {
                String tmp = @"/Users/andreymakarov/Downloads/SPOlab1/input.txt";
                sw = new StreamWriter(tmp);
            }

            Random rand = new Random();
            for (int i = 0; i < 3; i++)
            {
                Process process = new Process(i + 1,
                                              rand.Next(0, 10),
                                              rand.Next(1, 10),
                                              0
                );
                sw.WriteLine(process.number.ToString() + " " + process.readinessTime.ToString() + " " +
                                         process.requiredAmount.ToString() + " " + process.priority.ToString());
                sw.Flush();
            }
            sw.Close();
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

            Tracer.AnyTrace("Исходные процессы:\n");
            while (!sr.EndOfStream)
            {
                String[] tmp = sr.ReadLine().Split();
                Process process = new Process(Convert.ToInt32(tmp[0]),
                                              Convert.ToInt32(tmp[1]),
                                              Convert.ToInt32(tmp[2]),
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
                                              process.priority));
            }
        }

        void GenerateFile2()
        {
            StreamWriter sw;
            if (Environment.OSVersion.Platform.ToString() == "Win32NT")
            {
                sw = new StreamWriter(@"D:\Langs\C#\SPOlab1\input2.txt");

            }
            else
            {
                String tmp = @"/Users/andreymakarov/Downloads/SPOlab1/input2.txt";
                sw = new StreamWriter(tmp);
            }

            for (int i = 0; i < processes.Count; i++)
            {
                sw.WriteLine(processes[i].number.ToString() + " " + processes[i].readinessTime.ToString() + " " +
                                         processes[i].requiredAmount.ToString() + " " + processes[i].priority.ToString());
                sw.Flush();
                allProcesses.Add(new Process(processes[i].number, processes[i].readinessTime,
                                         processes[i].requiredAmount, processes[i].priority));
            }

            Random rand = new Random();
            for (int i = 0; i < maxNCP - processes.Count; i++)
            {
               
                int index = rand.Next(0, processes.Count);


                sw.WriteLine(processes[index].number + " " +
                             processes[index].readinessTime + " " +
                             processes[index].requiredAmount + " " +
                             processes[index].priority);
                sw.Flush();
                allProcesses.Add(new Process(processes[index].number, processes[index].readinessTime,
                                             processes[index].requiredAmount, processes[index].priority));

                allSjfProcesses.Add(new Process(processes[index].number, processes[index].readinessTime,
                             processes[index].requiredAmount, processes[index].priority));
            }
        }

        //Действия по окончанию прогона
        public override void FinishModelling(int variantCount, int runCount)
        {
            Tracer.AnyTrace("");
            Tracer.TraceOut("==============================================================");
            Tracer.TraceOut("============Статистические результаты моделирования===========");
            Tracer.TraceOut("==============================================================");
            Tracer.AnyTrace("");

            Tracer.AnyTrace("Суммарное количество затраченных тактов процессора для FIFO: " + (fifoTickNumber));
            Tracer.AnyTrace("Суммарное количество затраченных тактов процессора для SJF: " + (tickNumber));
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

