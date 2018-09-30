using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonModel.Kernel;

namespace Model_Lab
{
    public partial class SmoModel : Model
    {
        // Класс события: срабатывание такта процессора для FIFO
        public class FIFO : TimeModelEvent<SmoModel>
        {
            // алгоритм обработки события            
            protected override void HandleEvent(ModelEventArgs args)
            {
                while (Model.QFIFO.Count < Model.processes.Count && Model.allProcesses.Count > 0)
                {
                    //Model.Tracer.AnyTrace("Процесс №" + (Model.allProcesses[0].number) + " добавлен в очередь" + 
                    //" " + (Model.allProcesses[0].readinessTime) + " " + (Model.allProcesses[0].requiredAmount));
                    Model.QFIFO.Add(new Process(Model.allProcesses[0].number,
                                                /*Model.measureNumber*/ +Model.allProcesses[0].readinessTime,
                                                Model.allProcesses[0].requiredAmount,
                                                Model.allProcesses[0].priority));
                    Model.allProcesses.RemoveAt(0);
                }

                if (Model.QFIFO.Count != 0)
                {
                    String outString = Model.measureNumber.ToString();
                    for (int i = 0; i < Model.processes.Count; i++)
                    {
                        bool isProcess = false;
                        for (int j = 0; j < Model.QFIFO.Count; j++)
                        {
                            if (Model.QFIFO[j].readinessTime <= Model.measureNumber)
                            {
                                if (Model.processes[i].number == Model.QFIFO[j].number)
                                {
                                    //Model.Tracer.AnyTrace("123");
                                    if (j == 0)
                                    {
                                        isProcess = true;
                                        outString += " В";
                                        break;
                                    }
                                    else
                                    {
                                        isProcess = true;
                                        outString += " Г";
                                        break;
                                    }
                                }
                                else
                                {
                                    //outString += " -";
                                    //Model.Tracer.AnyTrace(" " + Model.processes[i].number + " " + Model.QFIFO[j].number);
                                }
                            }
                            else
                            {
                                isProcess = true;
                                outString += " Б";
                                break;
                            }
                        }
                        if (!isProcess)
                        {
                            outString += " -";
                        }
                    }
                    Model.Tracer.AnyTrace(outString);
                }
                /* Adding expected process in queue */
                //Model.Tracer.AnyTrace("Такт №" + Model.measureNumber);
                //int j = 0;
                //if (Model.allProcesses.Count > 0)
                //{
                //    while (j < Model.allProcesses.Count)
                //    {
                //        if (Model.allProcesses[j].readinessTime <= Model.measureNumber)
                //        {
                //            Model.Tracer.AnyTrace("Процесс №" + (Model.allProcesses[j].number) + " добавлен в очередь" + 
                //                                  " " + (Model.allProcesses[j].readinessTime) + " " + (Model.allProcesses[j].requiredAmount));

                //            Model.QFIFO.Add(
                //            new Process(Model.allProcesses[j].number,
                //                        Model.allProcesses[j].readinessTime,
                //                        Model.allProcesses[j].requiredAmount,
                //                        Model.allProcesses[j].priority));
                //            Model.allProcesses.RemoveAt(j);

                //            j--;
                //        }
                //        j++;
                //    }
                //}

                if (Model.QFIFO[0].readinessTime <= Model.measureNumber)
                {
                    Model.QFIFO[0].requiredAmount--;
                    /* First process is completed */
                    if (Model.QFIFO[0].requiredAmount == 0)
                    {
                        Model.QFIFO.RemoveAt(0);
                        Model.NCP++;

                        //if (Model.allProcesses.Count > 0)
                        //{
                        //    Random rand = new Random();
                        //    int index = rand.Next(0, Model.processes.Count);

                        //    //Model.Tracer.AnyTrace("GПроцесс №" + (Model.processes[index].number) + " добавлен в очередь" +
                        //    //" " + (Model.processes[index].readinessTime) + " " + (Model.processes[index].requiredAmount));

                        //    Model.waitProcesses.Add(new Process(Model.allProcesses[index].number,
                        //                                        Model.measureNumber + Model.allProcesses[index].readinessTime,
                        //                                        Model.allProcesses[index].requiredAmount,
                        //                                        Model.allProcesses[index].priority));

                        //    //Model.waitProcesses.Last().readinessTime = Model.measureNumber + Model.processes[index].readinessTime;
                        //}
                    }
                }

                    /* Printing queue */
                    //for (int i = 0; i < Model.QFIFO.Count; i++)
                    //{
                    //    Model.Tracer.AnyTrace(Model.QFIFO[i].number + " " + Model.QFIFO[i].readinessTime + " " + Model.QFIFO[i].requiredAmount);
                    //}

                    //Model.Tracer.AnyTrace(Model.VQ[0].requiredAmount);

                Model.measureNumber++;

                if (Model.NCP < Model.maxNCP)
                {
                    var ev = new FIFO();
                    Model.PlanEvent(ev, 1.0);
                    //Model.Tracer.PlanEventTrace(ev);
                    Model.Tracer.AnyTrace("");
                }
                else
                {
                    var ev = new SJF();
                    Model.PlanEvent(ev, 1.0);
                    Model.Tracer.AnyTrace("// SJF //\n");

                    Model.waitProcesses.Clear();
                    Model.fifoMeasureNumber = Model.measureNumber;
                    Model.measureNumber = 0;
                    Model.NCP = 0;

                    foreach (Process process in Model.processes)
                    {
                        Model.waitProcesses.Add(new Process(process.number,
                                                      process.readinessTime,
                                                      process.requiredAmount,
                                                      process.priority));
                    }
                }
            }
        }

        // Класс события: срабатывание такта процессора для SJF
        public class SJF : TimeModelEvent<SmoModel>
        {
            // алгоритм обработки события            
            protected override void HandleEvent(ModelEventArgs args)
            {
                //Model.Tracer.AnyTrace("Такт №" + Model.measureNumber);
                /* Выбор первого процесса с временем готовности меньшим либо равным текущему такту*/
                int i = 0;
                int minLength = -1;
                int minProcNumber = -1;
                int minReadinessTime = -1;
                for (i = 0; i < Model.waitProcesses.Count; i++)
                {
                    if (Model.waitProcesses[i].readinessTime <= Model.measureNumber)
                    {
                        minLength = Model.waitProcesses[i].requiredAmount;
                        minProcNumber = i;
                        minReadinessTime = Model.waitProcesses[i].readinessTime;
                        break;
                    }
                }

                if (minLength != -1)
                {
                    /* Выбор процесса с минимальной длиной и таким же или меньшим временем готовности */
                    for (int j = i + 1; j < Model.waitProcesses.Count; j++)
                    {
                        if (Model.waitProcesses[j].readinessTime <= Model.measureNumber)
                        {
                            if (Model.waitProcesses[j].requiredAmount < minLength)
                            {
                                minLength = Model.waitProcesses[j].requiredAmount;
                                minProcNumber = j;
                                minReadinessTime = Model.waitProcesses[j].readinessTime;
                            }
                            else if ((Model.waitProcesses[j].requiredAmount == minLength && Model.waitProcesses[j].readinessTime < minReadinessTime))
                            {
                                minLength = Model.waitProcesses[j].requiredAmount;
                                minProcNumber = j;
                                minReadinessTime = Model.waitProcesses[i].readinessTime;
                            }
                        }
                    }

                    /*  */
                    for (int j = 0; j < Model.waitProcesses.Count; j++)
                    {
                        if (Model.waitProcesses[j].readinessTime <= Model.measureNumber)
                        {
                            if ((Model.waitProcesses[j].requiredAmount == minLength && Model.waitProcesses[j].readinessTime < minReadinessTime))
                            {
                                minLength = Model.waitProcesses[j].requiredAmount;
                                minProcNumber = j;
                                minReadinessTime = Model.waitProcesses[j].readinessTime;
                            }
                        }
                    }

                    //Model.Tracer.AnyTrace(minProcNumber + " " + (Model.waitProcesses[0].number - 1));
                    if (Model.waitProcesses.Count != 0)
                    {
                        String outString = Model.measureNumber.ToString();
                        //bool isFirstMinProc = true;
                        //int newMinProcNumber = Model.waitProcesses[minProcNumber].number - 1;

                        //Model.Tracer.AnyTrace(minProcNumber + " " + newMinProcNumber);
                        for (int j = 0; j < Model.processes.Count; j++)
                        {
                            bool isProcess = false;
                            for (int k = 0; k < Model.waitProcesses.Count; k++)
                            {
                                bool isBlockedProcess = true;
                                if (Model.waitProcesses[k].readinessTime <= Model.measureNumber)
                                {
                                    Model.Tracer.AnyTrace(Model.processes[j].number + " " + Model.waitProcesses[k].number);
                                    if (Model.processes[j].number == Model.waitProcesses[k].number)
                                    {
                                        Model.Tracer.AnyTrace(Model.waitProcesses[minProcNumber].number - 1 + " " + k + " " + minProcNumber);
                                        if (k == Model.waitProcesses[minProcNumber].number - 1)
                                        {
                                            outString += " В";
                                            isProcess = true;
                                            isBlockedProcess = false;
                                            break;
                                        }
                                        else
                                        {
                                            outString += " Г";
                                            isProcess = true;
                                            isBlockedProcess = false;
                                            break;
                                        }
                                    }
                                }
                                //if (isBlockedProcess)
                                //{
                                //    outString += " Б";
                                //    isProcess = true;
                                //    //break;
                                //}
                            }

                            if (!isProcess)
                            {
                                outString += " Б";
                            }
                        }
                        Model.Tracer.AnyTrace(outString);
                    }


                    Model.waitProcesses[minProcNumber].requiredAmount--;
                    if (Model.waitProcesses[minProcNumber].requiredAmount == 0)
                    {
                        Model.waitProcesses.RemoveAt(minProcNumber);
                        Model.NCP++;
                        //Random rand = new Random();
                        //int index = rand.Next(0, Model.processes.Count);

                        if (Model.allSjfProcesses.Count > 0)
                        {
                            Model.waitProcesses.Add(new Process(Model.allSjfProcesses[0].number,
                                                                Model.allSjfProcesses[0].readinessTime,
                                                                Model.allSjfProcesses[0].requiredAmount,
                                                                Model.allSjfProcesses[0].priority));
                            Model.allSjfProcesses.RemoveAt(0);

                            //Model.waitProcesses.Last().readinessTime = Model.measureNumber + Model.processes[0].readinessTime;

                            //Model.Tracer.AnyTrace("Процесс №" + (Model.waitProcesses.Last().number) + " добавлен в очередь" +
                            //" " + (Model.waitProcesses.Last().readinessTime) + " " + (Model.waitProcesses.Last().requiredAmount));
                        }
                    }
                }
                else
                {
                    Model.Tracer.AnyTrace(Model.measureNumber.ToString() + " - - -");
                }

                /* Printing queue */
                for (int k = 0; k < Model.waitProcesses.Count; k++)
                {
                    Model.Tracer.AnyTrace(Model.waitProcesses[k].number + " " + Model.waitProcesses[k].readinessTime + " " + Model.waitProcesses[k].requiredAmount);
                }

                //Model.Tracer.AnyTrace(Model.waitProcesses.Count + " " + minProcNumber);


                Model.measureNumber++;

                if (Model.NCP < Model.maxNCP)
                {
                    var ev = new SJF();
                    Model.PlanEvent(ev, 1.0);
                    //Model.Tracer.PlanEventTrace(ev);
                    Model.Tracer.AnyTrace("");
                }
                else
                {
                    //Model.measureNumber--;
                    Model.isFinish = true;
                }
            }
        }
    }
}
