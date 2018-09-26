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
                int j = 0;
                while (j < Model.waitProcesses.Count)
                { 
                    if (Model.waitProcesses[j].readinessTime >= Model.measureNumber)
                    {
                        Model.Tracer.AnyTrace("Процесс №" + (Model.waitProcesses[j].number + 1) + " добавлен в очередь");
                        Model.VQ.Add(Model.waitProcesses[j]);
                        Model.waitProcesses.RemoveAt(j);
                        j--;
                    }
                }

                Model.VQ[0].requiredAmount--;
                if (Model.VQ[0].requiredAmount == 0)
                {

                    Model.VQ.RemoveAt(0);
                    Model.NCP++;
                    Random rand = new Random();
                    int index = rand.Next(0, Model.processes.Count);
                    Model.waitProcesses.Add(Model.processes[index]);
                    //Model.VQ.Last().readinessTime = Model.measureNumber + Model.processes[index].readinessTime;
                    //Model.Tracer.AnyTrace("//////////////NCP = " + Model.NCP);
                    //Model.Tracer.AnyTrace("Процесс №" + (index + 1) + " добавлен в очередь");
                }

                for (int i = 0; i < Model.VQ.Count; i++)
                {
                    Model.Tracer.AnyTrace(Model.VQ[i].number + " " + Model.VQ[i].readinessTime + " " + Model.VQ[i].requiredAmount);
                }

                Console.ReadLine();

                Model.Tracer.AnyTrace(Model.VQ[0].requiredAmount);

                if (Model.NCP < Model.maxNCP-1)
                {
                    var ev = new FIFO();
                    Model.PlanEvent(ev, 1.0);
                    Model.Tracer.PlanEventTrace(ev);

                    Model.measureNumber++;
                }
                else
                {
                    var ev = new ProcessCreation();
                    Model.PlanEvent(ev, 1.0);
                    Model.Tracer.PlanEventTrace(ev);
                }
            }
        }

        // Класс события: Создание нового процесса
        public class ProcessCreation : TimeModelEvent<SmoModel>
        {
            //#region Атрибуты события
            //public Bid ZP;
            //#endregion

            // алгоритм обработки события            
            protected override void HandleEvent(ModelEventArgs args)
            {
                //Model.Tracer.EventTrace(this);

                //Random rnd = new Random();
                //while (Model.currN < Model.maxN)
                //{
                //    Model.currN++;
                //    Process process = new Process(Model.currN, rnd.Next(Model.minNT, Model.maxNT), 0,);
                //    var rec = new QRec(process);
                //    Model.VQ.Add(rec);
                //    Model.Tracer.AnyTrace("Добавлен процесс №" + Model.currN + "\tТакты: " + process.RNT);
                //}
            }
        }
    }
}
