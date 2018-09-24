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
                for (int i = 0; i < Model.processes.Count; i++)
                {
                    if (Model.processes[i].readinessTime == Model.measureNumber)
                    {
                        Model.Tracer.AnyTrace("Процесс №" + (i+1) + " добавлен в очередь");
                        Model.VQ.Add(Model.processes[i]);
                    }
                }

                Model.VQ[0].requiredAmount--;
                if (Model.VQ[0].requiredAmount == 0)
                {
                    Model.VQ.RemoveAt(0);
                    Model.NCP++;
                }

                Model.Tracer.AnyTrace(Model.VQ[0].requiredAmount);

                var ev = new FIFO();
                Model.PlanEvent(ev, 1.0);
                Model.Tracer.PlanEventTrace(ev);

                Model.measureNumber++;
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
