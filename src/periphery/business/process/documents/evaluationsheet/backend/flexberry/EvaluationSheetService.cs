namespace Tmobis.Business.Process.Documents;

using Tmobis.Business.Common.Contacts;
using Tmobis.Core.Repositories;

public class EvaluationSheetService(
    IRepository repository)
{
    Dictionary<string, List<string>> dictCalculation = new()
    {
        { "1", [ "2", "3", "4" ]},
        { "4", [ "9", "46" ]},
        { "9", [ "10", "26", "29", "30", "31" ]},
        { "10", [ "11", "12", "13", "16", "17", "20", "21", "24", "25" ]},
        { "13", [ "14", "15" ]},
        { "17", [ "18", "19" ]},
        { "21", [ "22", "23" ]},
        { "26", [ "27", "28" ]},
        { "31", [ "32", "35", "36", "37", "38", "39", "40", "41", "42", "45" ]},
        { "32", [ "33", "34" ]},
        { "47", [ "48", "67", "70", "71", "72", "73", "94", "95" ]},
        { "48", [ "49", "50", "53", "54", "57", "58", "61", "62", "65", "66" ]},
        { "54", [ "55", "56" ]},
        { "58", [ "59", "60" ]},
        { "62", [ "63", "64" ]},
        { "67", [ "68", "69" ]},
        { "73", [ "74", "77", "78", "79", "80", "81", "82", "83", "84", "85", "90", "93" ]},
        { "74", [ "75", "76" ]},
        { "85", [ "86", "87" ]},
        { "98", [ "99", "115", "118", "119", "120", "134" ]},
        { "99", [ "100", "101", "102", "105", "106", "109", "110", "113", "114" ]},
        { "102", [ "103", "104" ]},
        { "106", [ "107", "108" ]},
        { "110", [ "111", "112" ]},
        { "115", [ "116", "117" ]},
        { "120", [ "121", "124", "125", "126", "127", "128", "129", "130", "131", "134" ]},
        { "121", [ "122", "123" ]},
        { "138", [ "139", "150", "153", "156" ]},
        { "139", [ "140", "141", "142", "143", "144", "145", "146", "147", "148", "149" ]},
        { "150", [ "151", "152" ]},
        { "153", [ "154", "155" ]},
        { "156", [ "157", "158" ]},
        { "159", [ "160", "161" ]},
        { "162", [ "163", "164" ]}
    };

    public decimal GetCountActs(EvaluationSheet evaluationSheet)
    {
        return repository.Query<RedinessActBase>().Where(x => x.ActStatus.Id == IActStatus.Keys.Signed &&
            x.Organization.Hierarchy.Id == evaluationSheet.Organization.Id &&
            x.Awp.Id == evaluationSheet.Awp.Id &&
            (x.Organization.OrganizationType.Id == OrganizationType.Keys.NotTco ||
            x.Organization.OrganizationType.Id == OrganizationType.Keys.Tco)).Count();
    }

    public decimal GetCountOrganizations(EvaluationSheet evaluationSheet)
    {
        return repository.Query<Organization>().Where(x => x.Hierarchy.Id == evaluationSheet.Organization.Id && x.Actuality &&
            (x.OrganizationType.Id == OrganizationType.Keys.NotTco ||
            x.OrganizationType.Id == OrganizationType.Keys.Tco)).Count();
    }

 
    public double RecountObjectReadinessLevel(object key, EvaluationSheet evaluationSheet)
    {
        double result = 0;
        if (key.Equals(OrganizationType.Keys.Other))
        {
            result = RecountReqOrders(new List<string> { "138", "159", "162", "165", "166" }, evaluationSheet);
        }
        if (key.Equals(OrganizationType.Keys.Municipality))
        {
            result = RecountReqOrders(new List<string> { "1", "5" }, evaluationSheet);
        }
        if (key.Equals(OrganizationType.Keys.NotTco))
        {
            result = RecountReqOrders(new List<string> { "98", "136", "137" }, evaluationSheet);
        }
        if (key.Equals(OrganizationType.Keys.Tco))
        {
            result = RecountReqOrders(new List<string> { "47", "96", "97" }, evaluationSheet);
        }
        return result;
    }

    private double RecountReqOrders(List<string> reqOrders, EvaluationSheet evaluationSheet)
    {
        return reqOrders.Sum(x => GetWeight(x, evaluationSheet) * RecountReqOrder(x, evaluationSheet));
    }

    private double GetWeight(string reqOrder, EvaluationSheet evaluationSheet)
    {
        int.TryParse(reqOrder, out var code);
        var evaluationSheetReqirement = evaluationSheet.Requirements
            .FirstOrDefault(x => x.ReqOrder == code);
        return evaluationSheetReqirement != null ?
            Convert.ToDouble(evaluationSheetReqirement.Weight) : 0;
    }

    private double RecountReqOrder(string reqOrder, EvaluationSheet evaluationSheet)
    {
        int.TryParse(reqOrder, out var code);
        float epsilon = 0.0001f;
        var evaluationSheetReqirement = evaluationSheet.Requirements
            .FirstOrDefault(x => x.ReqOrder == code);
        double result = 0;
        if (evaluationSheetReqirement != null)
        {
            if (dictCalculation.ContainsKey(reqOrder))
            {
                result = RecountReqOrders(dictCalculation[reqOrder], evaluationSheet);
            }
            else
            {
                switch (reqOrder)
                {
                    case "4":
                        if (!evaluationSheet.OwnerlessObjects)
                        {
                            result = 1;
                        }
                        break;
                    case "5":
                        result = (RecountReqOrder("8", evaluationSheet) > 0 + epsilon) ? RecountReqOrder("6", evaluationSheet) * (RecountReqOrder("7", evaluationSheet) / RecountReqOrder("8", evaluationSheet)) : 0;
                        break;
                    case "7":
                        result = (double)GetCountActs(evaluationSheet);
                        break;
                    case "8":
                        result = (double)GetCountOrganizations(evaluationSheet);
                        break;
                    case "42":
                        result = (RecountReqOrder("44", evaluationSheet) > 0 + epsilon) ? RecountReqOrder("43", evaluationSheet) / RecountReqOrder("44", evaluationSheet) : 0;
                        break;
                    case "50":
                        result = (RecountReqOrder("52", evaluationSheet) > 0 + epsilon) ? RecountReqOrder("51", evaluationSheet) / RecountReqOrder("52", evaluationSheet) : 0;
                        break;
                    case "87":
                        result = RecountReqOrder("88", evaluationSheet) >= RecountReqOrder("89", evaluationSheet) ? 1 : 0;
                        break;
                    case "90":
                        result = (RecountReqOrder("92", evaluationSheet) > 0 + epsilon) ? RecountReqOrder("91", evaluationSheet) / RecountReqOrder("92", evaluationSheet) : 0;
                        break;
                    case "131":
                        result = (RecountReqOrder("133", evaluationSheet) > 0 + epsilon) ? RecountReqOrder("132", evaluationSheet) / RecountReqOrder("133", evaluationSheet) : 0;
                        break;
                    default:
                        result = (double)evaluationSheetReqirement.CalculatedReq;
                        break;
                }
            }
            evaluationSheetReqirement.CalculatedReq = (decimal)result;
        }
        return result;
    }

    public LevelReadiness GetStatus(double? recount, object key, EvaluationSheet evaluationSheet)
    {
        LevelReadiness status = LevelReadiness.NotReady;
        float epsilon = 0.0001f;
        if (recount >= 0.8 && recount < 0.9)
        {
            status = LevelReadiness.ReadyWithConditions;
        }
        if (recount >= 0.9)
        {
            status = LevelReadiness.Ready;
            if (key.Equals(OrganizationType.Keys.Other))
            {
                if ((RecountReqOrder("140", evaluationSheet) <= 0 + epsilon) ||
                    (RecountReqOrder("141", evaluationSheet) <= 0 + epsilon))
                {
                    status = LevelReadiness.ReadyWithConditions;
                }
            }
            if (key.Equals(OrganizationType.Keys.NotTco))
            {
                if (RecountReqOrder("126", evaluationSheet) <= 0 + epsilon)
                {
                    status = LevelReadiness.ReadyWithConditions;
                }
            }
            if (key.Equals(OrganizationType.Keys.Tco))
            {
                if (RecountReqOrder("82", evaluationSheet) <= 0 + epsilon)
                {
                    status = LevelReadiness.ReadyWithConditions;
                }
            }
        }
        return status;
    }
}
