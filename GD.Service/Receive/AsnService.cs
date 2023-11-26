using GD.Infrastructure.Attribute;
using GD.Model.Basic;
using GD.Model.Dto.Receive;
using GD.Model.Enums;
using GD.Model.Page;
using GD.Model.Receive;
using GD.Repository;
using GD.Service.Interface.Receive;
using SqlSugar;

namespace GD.Service.Receive
{
    [AppService(ServiceType = typeof(IAsnService), ServiceLifetime = LifeTime.Transient)]
    public class AsnService : BaseService<Asn>, IAsnService
    {
        public int Add(Asn asn, string userName)
        {
            asn.Create_by = userName;
            asn.Create_time = DateTime.Now;
            asn.SupplierName = Context
                .Queryable<Supplier>()
                .First(it => it.SupplierId == asn.SupplierId)
                .SupplierName;
            asn.OwnerName = Context
                .Queryable<Owner>()
                .First(it => it.OwnerId == asn.OwnerId)
                .OwnerName;
            var data = Context
                .Queryable<CommoditySKU>()
                .LeftJoin<CommoditySPU>((sku, spu) => sku.CommoditySPUId == spu.CommoditySPUId)
                .Where(sku => sku.CommoditySKUId == asn.SkuId)
                .Select((sku, spu) => new
                {
                    spu.LengthUnit,
                    spu.WeightUnit,
                    spu.VolumeUnit,
                    sku.Weight,
                    sku.Volume
                })
                .First();
            asn.LengthUnit = data.LengthUnit;
            asn.WeightUnit = data.WeightUnit;
            asn.VolumeUnit = data.VolumeUnit;
            asn.Weight = data.Weight;
            asn.Volume = data.Volume;
                
            string code = "";
            string date = DateTime.Now.ToString("yyyy" + "MM" + "dd");
            var asnNo = Queryable()
                .OrderBy(it => it.AsnId, OrderByType.Desc)
                .First()
                .AsnNo
                .Split("-");

            if (asnNo.First().Equals(date))
            {
                var newNo = int.Parse(asnNo.Last()) + 1;
                code = $"{date}-{newNo.ToString("0000")}";
            }
            else
            {
                code = $"{date}-0001";
            }
            asn.AsnNo = code;

            return Insert(asn);
        }

        public int Delete(long asnId)
        {
            return base.Delete(asnId);
        }

        public int Edit(Asn asn, string userName)
        {
            asn.Update_by = userName;
            asn.Update_time = DateTime.Now;
            asn.SupplierName = Context
                .Queryable<Supplier>()
                .First(it => it.SupplierId == asn.SupplierId)
                .SupplierName;
            asn.OwnerName = Context
                .Queryable<Owner>()
                .First(it => it.OwnerId == asn.OwnerId)
                .OwnerName;

            return Update(asn);
        }

        public Asn Get(long asnId)
        {
            return Queryable()
                .First(asn => asn.AsnId == asnId);
        }

        public PagedInfo<Asn> GetAll(AsnQueryDto asnQueryDto)
        {
            var expression = Expressionable.Create<Asn>()
                .AndIF(!string.IsNullOrEmpty(asnQueryDto.SkuName), asn => asn.SkuName.Contains(asnQueryDto.SkuName))
                .AndIF(!string.IsNullOrEmpty(asnQueryDto.SupplierName), asn => asn.SupplierName.Contains(asnQueryDto.SupplierName))
                .AndIF(asnQueryDto.AsnStatus != null, asn => asn.AsnStatus == asnQueryDto.AsnStatus)
                .AndIF(asnQueryDto.BeginTime != DateTime.MinValue && asnQueryDto.BeginTime != null, exp => exp.Create_time >= asnQueryDto.BeginTime)
                .AndIF(asnQueryDto.EndTime != DateTime.MaxValue && asnQueryDto.EndTime != null, exp => exp.Create_time <= asnQueryDto.EndTime);

            return Queryable()
                .Where(expression.ToExpression())
                .ToPage(asnQueryDto);
        }

        public List<Asn> GetAll(AsnStatus? asnStatus = null)
        {
            return Queryable()
                .WhereIF(asnStatus != null, it => it.AsnStatus == ((int?)asnStatus))
                .ToList();
        }

        public int Operate(long asnId, AsnStatus asnStatus, string userName)
        {
            return Context
                .Updateable<Asn>()
                .SetColumns(asn => new Asn { AsnStatus = (int)asnStatus, Update_by = userName, Update_time = DateTime.Now })
                .Where(asn => asn.AsnId == asnId)
                .ExecuteCommand();
        }
    }
}
