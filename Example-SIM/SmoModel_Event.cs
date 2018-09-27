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
                /* Adding expected process in queue */
                Model.Tracer.AnyTrace("Такт №" + Model.measureNumber);
                int j = 0;
                if (Model.waitProcesses.Count > 0)
                {
                    while (j < Model.waitProcesses.Count)
                    {
                        if (Model.waitProcesses[j].readinessTime <= Model.measureNumber)
                        {
                            Model.Tracer.AnyTrace("Процесс №" + (Model.waitProcesses[j].number) + " добавлен в очередь" + 
                                                  " " + (Model.waitProcesses[j].readinessTime) + " " + (Model.waitProcesses[j].requiredAmount));

                            Model.QFIFO.Add(
                            new Process(Model.waitProcesses[j].number,
                                        Model.waitProcesses[j].readinessTime,
                                        Model.waitProcesses[j].requiredAmount,
                                              0,
                                        Model.waitProcesses[j].priority));
                            Model.waitProcesses.RemoveAt(j);

                            j--;
                        }
                        j++;
                    }
                }

                Model.QFIFO[0].requiredAmount--;
                /* First process is completed */
                if (Model.QFIFO[0].requiredAmount == 0)
                {
                    Model.QFIFO.RemoveAt(0);
                    Model.NCP++;
                    Random rand = new Random();
                    int index = rand.Next(0, Model.processes.Count);

                    //Model.Tracer.AnyTrace("GПроцесс №" + (Model.processes[index].number) + " добавлен в очередь" +
                      //" " + (Model.processes[index].readinessTime) + " " + (Model.processes[index].requiredAmount));

                    Model.waitProcesses.Add(new Process(Model.processes[index].number,
                                                        Model.processes[index].readinessTime,
                                                        Model.processes[index].requiredAmount,
                                                        0,
                                                        Model.processes[index].priority));

                    Model.waitProcesses.Last().readinessTime = Model.measureNumber + Model.processes[index].readinessTime;
                }

                /* Printing queue */
                for (int i = 0; i < Model.QFIFO.Count; i++)
                {
                    Model.Tracer.AnyTrace(Model.QFIFO[i].number + " " + Model.QFIFO[i].readinessTime + " " + Model.QFIFO[i].requiredAmount);
                }

                //Model.Tracer.AnyTrace(Model.VQ[0].requiredAmount);

                if (Model.NCP < Model.maxNCP-1)
                {
                    var ev = new FIFO();
                    Model.PlanEvent(ev, 1.0);
                    //Model.Tracer.PlanEventTrace(ev);
                    Model.Tracer.AnyTrace("");
                    Model.measureNumber++;
                }
                else
                {
                    var ev = new SJF();
                    Model.PlanEvent(ev, 1.0);
                    Model.Tracer.PlanEventTrace(ev);

                    Model.waitProcesses.Clear();
                    Model.fifoMeasureNumber = Model.measureNumber;
                    Model.measureNumber = 0;
                    Model.NCP = 0;

                    foreach (Process process in Model.processes)
                    {
                        Model.waitProcesses.Add(new Process(process.number,
                                                      process.readinessTime,
                                                      process.requiredAmount,
                                                      0,
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
                Model.Tracer.AnyTrace("Такт №" + Model.measureNumber);

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

                    Model.waitProcesses[minProcNumber].requiredAmount--;
                    if (Model.waitProcesses[minProcNumber].requiredAmount == 0)
                    {
                        Model.waitProcesses.RemoveAt(minProcNumber);
                        Model.NCP++;
                        Random rand = new Random();
                        int index = rand.Next(0, Model.processes.Count);

                        Model.waitProcesses.Add(new Process(Model.processes[index].number,
                                                            Model.processes[index].readinessTime,
                                                            Model.processes[index].requiredAmount,
                                                            0,
                                                            Model.processes[index].priority));

                        Model.waitProcesses.Last().readinessTime = Model.measureNumber + Model.processes[index].readinessTime;

                        Model.Tracer.AnyTrace("Процесс №" + (Model.waitProcesses.Last().number) + " добавлен в очередь" +
                                              " " + (Model.waitProcesses.Last().readinessTime) + " " + (Model.waitProcesses.Last().requiredAmount));
                    }
                }

                /* Printing queue */
                for (int k = 0; k < Model.waitProcesses.Count; k++)
                {
                    Model.Tracer.AnyTrace(Model.waitProcesses[k].number + " " + Model.waitProcesses[k].readinessTime + " " + Model.waitProcesses[k].requiredAmount);
                }

                Model.measureNumber++;

                if (Model.NCP < Model.maxNCP - 1)
                {
                    var ev = new SJF();
                    Model.PlanEvent(ev, 1.0);
                    //Model.Tracer.PlanEventTrace(ev);
                    Model.Tracer.AnyTrace("");
                }
                else
                {
                    Model.isFinish = true;
                }
            }
        }
    }
}
