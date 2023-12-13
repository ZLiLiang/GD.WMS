using GD.Infrastructure.Attribute;
using GD.Model.Basic;
using GD.Model.Dto.Operation;
using GD.Model.Inventory;
using GD.Model.Operation;
using GD.Model.Page;
using GD.Model.Vm.Operation;
using GD.Repository;
using GD.Service.Interface.Inventory;
using GD.Service.Interface.Operation;
using Mapster;
using MapsterMapper;
using SqlSugar;
using System.Linq.Expressions;

namespace GD.Service.Operation
{
    [AppService(ServiceType = typeof(IProcessService), ServiceLifetime = LifeTime.Transient)]
    public class ProcessService : BaseService<Process>, IProcessService
    {
        public int Add(ProcessDto processDto, string userName)
        {
            var config = new TypeAdapterConfig();
            config.ForType<ProcessDto, Process>()
                .Map(src => src.ProcessDetails, dest => dest.DetailList)
                .Ignore(dto => dto.JobCode)
                .PreserveReference(true);
            config.ForType<ProcessDetailDto, ProcessDetail>()
                .PreserveReference(true);
            var mapper = new Mapper(config);

            var result = mapper.Map<Process>(processDto);

            result.Create_by = userName;
            result.JobCode = GetProcessJobCode();
            return Context
                .InsertNav(result)
                .Include(it => it.ProcessDetails)
                .ExecuteCommand() ? 1 : 0;

        }

        public int ConfirmAdjustment(long processId, string userName)
        {
            var process = Context.Queryable<Process>()
                .First(it => it.ProcessId == processId);
            if (process == null)
            {
                return 0;
            }
            var adjusted = Context.Queryable<Adjust>()
                .LeftJoin<ProcessDetail>((adj, proDet) => adj.SourceTableId == proDet.ProcessDetailId)
                .Where((adj, proDet) => adj.JobType == 2 && proDet.ProcessId == processId)
                .Any();
            if (process.ProcessStatus == 1 && adjusted)
            {
                return 0;
            }
            var details = Context.Queryable<ProcessDetail>()
                .Where(it => it.ProcessId == processId)
                .ToList();
            var adjusts = details
                .Select(it => new Adjust
                {
                    SkuId = it.SkuId,
                    SourceTableId = it.ProcessDetailId,
                    IsUpate = 1,
                    LocationId = it.LocationId,
                    JobType = 2,
                    OwnerId = it.OwnerId,
                    Qty = it.IsSource == 0 ? -it.Qty : it.Qty,
                    Create_by = userName
                })
                .ToList();
            process.Update_by = userName;
            //可能出现问题的地方
            var stocks = Context.Queryable<Stock>()
                .Where(it => SqlFunc.Subqueryable<ProcessDetail>()
                .Where(proDet => proDet.ProcessId == processId)
                .Where(proDet => proDet.LocationId == it.LocationId && proDet.SkuId == it.SkuId && proDet.OwnerId == it.OwnerId)
                .Any())
                .ToList();


            foreach (var item in details)
            {
                var stock = stocks.FirstOrDefault(it => it.LocationId == item.LocationId && it.SkuId == item.SkuId && it.OwnerId == item.OwnerId);
                item.IsUpate = 1;
                if (item.IsSource == 0)
                {
                    if (stock == null)
                    {
                        return 0;
                    }
                    stock.Qty -= item.Qty;
                }
                else
                {
                    if (stock == null)
                    {
                        Context.Insertable(new Stock
                        {
                            SkuId = item.SkuId,
                            LocationId = item.LocationId,
                            OwnerId = item.OwnerId,
                            IsFreeze = 0,
                            Qty = item.Qty
                        }).ExecuteCommand();
                    }
                    else
                    {
                        stock.Qty += item.Qty;
                    }
                }
            }
            var code = GetAdjustJobCode();
            adjusts.ForEach(it => it.JobCode = code);
            return Context.Insertable(adjusts).ExecuteCommand();

        }

        public int ConfirmProcess(long processId, string userName)
        {
            var queryResult = Queryable()
                .First(it => it.ProcessId == processId);

            if (queryResult == null)
            {
                return 0;
            }
            if (queryResult.ProcessStatus == 1)
            {
                return 0;
            }
            queryResult.Processor = userName;
            queryResult.ProcessTime = DateTime.Now;
            queryResult.ProcessStatus = 1;

            return Update(queryResult);
        }

        public int Delete(long processId)
        {
            return Context
                .DeleteNav<Process>(it => it.ProcessId == processId)
                .Include(it => it.ProcessDetails)
                .ExecuteCommand() ? 1 : 0;
        }

        public int Edit(ProcessDto processDto, string userName)
        {
            var config = new TypeAdapterConfig();
            config.ForType<Process, ProcessDto>()
                .Map(dest => dest.DetailList, src => src.ProcessDetails);
            var mapper = new Mapper(config);

            var result = mapper.Map<Process>(processDto);

            result.Update_by = userName;
            return Context
                .UpdateNav(result)
                .Include(it => it.ProcessDetails)
                .ExecuteCommand() ? 1 : 0;
        }

        public ProcessVm Get(long processId)
        {
            var config = new TypeAdapterConfig();
            config.ForType<Process, ProcessVm>()
                .Map(dest => dest.AdjustStatus, src => src.ProcessStatus);
            var mapper = new Mapper(config);

            var queryRuselt = Queryable()
                .Includes(it => it.ProcessDetails)
                .Where(it => it.ProcessId == processId)
                .First();

            var reuslt = mapper.Map<ProcessVm>(queryRuselt);

            foreach (var item in queryRuselt.ProcessDetails)
            {
                var detail = item.Adapt<ProcessDetailVm>();
                detail.LocationCode = Context.Queryable<Location>()
                    .First(it => it.LocationId == item.LocationId)?
                    .LocationCode ?? string.Empty;
                var sku = Context.Queryable<CommoditySKU>()
                    .LeftJoin<CommoditySPU>((csku, cspu) => csku.CommoditySPUId == cspu.CommoditySPUId)
                    .Where(csku => csku.CommoditySKUId == item.SkuId)
                    .Select((csku, cspu) => new
                    {
                        skuCode = csku.CommoditySKUCode,
                        spuName = cspu.CommoditySPUName,
                        spuCode = cspu.CommoditySPUCode,
                        unit = csku.Unit
                    })
                    .First();
                detail.SkuCode = sku.skuCode;
                detail.SpuCode = sku.spuCode;
                detail.SpuName = sku.spuName;
                detail.Unit = sku.unit;

                if (item.IsSource == 0)
                {
                    reuslt.SourceDetailList.Add(detail);
                }
                else
                {
                    reuslt.TargetDetailList.Add(detail);
                }
            }

            return reuslt;

        }

        public string GetAdjustJobCode()
        {
            string code = "";
            string date = DateTime.Now.ToString("yyyy" + "MM" + "dd");
            var maxNo = Context
                .Queryable<Adjust>()
                .OrderBy(it => it.JobCode, OrderByType.Desc)
                .First()?
                .JobCode
                .Split("-");

            if (maxNo != null && maxNo.First().Equals(date))
            {
                var newNo = int.Parse(maxNo.Last()) + 1;
                code = $"{date}-{newNo:0000}"; //newNo.ToString("0000")
            }
            else
            {
                code = $"{date}-0001";
            }

            return code;
        }

        public PagedInfo<ProcessVm> GetAll(ProcessQueryDto processQueryDto)
        {
            var expression = Expressionable.Create<ProcessVm>()
                .AndIF(!string.IsNullOrEmpty(processQueryDto.JobCode), process => process.JobCode.Contains(processQueryDto.JobCode))
                .AndIF(processQueryDto.BeginTime != DateTime.MinValue && processQueryDto.BeginTime != null, exp => exp.Create_time >= processQueryDto.BeginTime)
                .AndIF(processQueryDto.EndTime != DateTime.MaxValue && processQueryDto.EndTime != null, exp => exp.Create_time <= processQueryDto.EndTime);

            var adjusted = Context.Queryable<Adjust>()
                .LeftJoin<ProcessDetail>((adj, proDet) => adj.SourceTableId == proDet.ProcessDetailId)
                .Where(adj => adj.JobType == 2)
                .GroupBy((adj, proDet) => proDet.ProcessId)
                .Select((adj, proDet) => new
                {
                    processId = proDet.ProcessId
                });

            return Queryable()
                .LeftJoin(adjusted, (it, adj) => it.ProcessId == adj.processId)
                .Select((it, adj) => new ProcessVm
                {
                    ProcessId = it.ProcessId,
                    JobCode = it.JobCode,
                    JobType = it.JobType,
                    ProcessStatus = it.ProcessStatus,
                    Processor = it.Processor,
                    ProcessTime = it.ProcessTime,
                    Create_by = it.Create_by,
                    Create_time = it.Create_time,
                    Update_by = it.Update_by,
                    Update_time = it.Update_time,
                    AdjustStatus = (it.ProcessStatus == 1 && (adj.processId == null ? false : true)) ? 1 : 0
                }, true)
                .Where(expression.ToExpression())
                .ToPage(processQueryDto);

        }
        public List<ProcessVm> GetAll()
        {
            //var adjusted = Context.Queryable<Adjust>()
            //    .LeftJoin<ProcessDetail>((adj, proDet) => adj.SourceTableId == proDet.ProcessDetailId)
            //    .Where(adj => adj.JobType == 2)
            //    .GroupBy((adj, proDet) => proDet.ProcessId)
            //    .Select((adj, proDet) => proDet.ProcessId)
            //    .ToList();

            //return Queryable()
            //    .Select(it => new ProcessVm
            //    {
            //        AdjustStatus = it.ProcessStatus == 1 && adjusted.Any(adj => adj == it.ProcessId) ? 1 : 0
            //    }, true)
            //    .ToList();
            var adjusted = Context.Queryable<Adjust>()
                .LeftJoin<ProcessDetail>((adj, proDet) => adj.SourceTableId == proDet.ProcessDetailId)
                .Where(adj => adj.JobType == 2)
                .GroupBy((adj, proDet) => proDet.ProcessId)
                .Select((adj, proDet) => new
                {
                    processId = proDet.ProcessId
                });

            return Queryable()
                .LeftJoin(adjusted, (it, adj) => it.ProcessId == adj.processId)
                .Select((it, adj) => new ProcessVm
                {
                    ProcessId = it.ProcessId,
                    JobCode = it.JobCode,
                    JobType = it.JobType,
                    ProcessStatus = it.ProcessStatus,
                    Processor = it.Processor,
                    ProcessTime = it.ProcessTime,
                    Create_by = it.Create_by,
                    Create_time = it.Create_time,
                    Update_by = it.Update_by,
                    Update_time = it.Update_time,
                    AdjustStatus = (it.ProcessStatus == 1 && (adj.processId == null ? false : true)) ? 1 : 0
                }, true)
                .ToList();
        }

        public string GetProcessJobCode()
        {
            string code = "";
            string date = DateTime.Now.ToString("yyyy" + "MM" + "dd");
            var maxNo = Queryable()
                .OrderBy(it => it.JobCode, OrderByType.Desc)
                .First()?
                .JobCode
                .Split("-");

            if (maxNo != null && maxNo.First().Equals(date))
            {
                var newNo = int.Parse(maxNo.Last()) + 1;
                code = $"{date}-{newNo:0000}"; //newNo.ToString("0000")
            }
            else
            {
                code = $"{date}-0001";
            }

            return code;
        }
    }
}
