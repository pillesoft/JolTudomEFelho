using JolTudomE_Api.Models;
using JolTudomE_Api.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace JolTudomE_Api.Controllers {
  [RoutePrefix("api/actstat")]
  [EnableCors("*", "*", "*")]
  public class ActionStatController : BaseController {
    [Route("get")]
    [AllowAnonymous]
    [HttpGet]
    public ActionStatResponse Statistic() {
      List<ActionStatistic> tresult = StorMan.GetData<ActionStatistic>(TStorageType.ActionStatistic);

      var q_label = from st in tresult
                    let nap = st.Mikor.ToString("yyyy-MM-dd")
                    group st by nap into g
                    orderby g.Key
                    select g.Key;

      var q_datasetlbl = from st in tresult
                      group st by st.PartitionKey into g
                      select g.Key;

      var q_data = from st in tresult
                   let nap = st.Mikor.ToString("yyyy-MM-dd")
                   group st by new { st.PartitionKey, nap } into g
                   orderby g.Key.PartitionKey, g.Key.nap
                   select new ActionStatResponseData { Action = g.Key.PartitionKey, When = g.Key.nap, HowMany = g.Count() };

      return new ActionStatResponse { Labels = q_label.ToList(), Actions = q_datasetlbl.ToList(), Data = q_data.ToList() };

    }

  }
}