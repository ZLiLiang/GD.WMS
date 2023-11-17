<template>
    <div class="app-container">
        <!-- 搜索表单 -->
        <el-form :model="queryParams" ref="queryRef" :inline="true" v-show="showSearch">
            <el-form-item label="商品编码" prop="commoditySPUCode">
                <el-input v-model="queryParams.commoditySPUCode" placeholder="请输入商品编码" clearable />
            </el-form-item>
            <el-form-item label="商品名称" prop="commoditySPUName">
                <el-input v-model="queryParams.commoditySPUName" placeholder="请输入商品名称" clearable />
            </el-form-item>
            <el-form-item label="商品类别" prop="categoryName">
                <el-input v-model="queryParams.categoryName" placeholder="请输入商品类别" clearable />
            </el-form-item>
            <el-form-item>
                <el-button type="primary" icon="search" @click="handleQuery">搜索</el-button>
                <el-button icon="refresh" @click="resetQuery">重置</el-button>
            </el-form-item>
        </el-form>

        <!-- 功能按钮 -->
        <el-row :gutter="10" class="mb8">
            <el-col :span="1.5">
                <el-button type="primary" plain icon="Plus" @click="handleAdd">新增</el-button>
            </el-col>
            <el-col :span="1.5">
                <el-button type="warning" plain icon="download" @click="handleExport">导出</el-button>
            </el-col>
            <el-col :span="1.5">
                <el-button color="#626aef" plain @click="handleQrCode">
                    <el-icon>
                        <SvgIcon name="qrcode" />
                    </el-icon>
                    <span> 二维码 </span>
                </el-button>
            </el-col>
            <el-col :span="1.5">
                <el-button color="#ff6a6a" plain @click="handleBarCode">
                    <el-icon>
                        <SvgIcon name="barcode" />
                    </el-icon>
                    <span> 条形码 </span>
                </el-button>
            </el-col>
            <right-toolbar v-model:showSearch="showSearch" @queryTable="getList"></right-toolbar>
        </el-row>

        <!-- 表格 -->
        <el-table ref="tableRef" v-loading="loading" :data="commoditySPUList" highlight-current-row border
            row-key="commoditySPUId">
            <el-table-column width="55" align="center">
                <template #header>
                    <el-checkbox v-model="checkAll" :indeterminate="isIndeterminate" @change="onTableSelectionChange">
                    </el-checkbox>
                </template>
                <template #default="scope">
                    <el-checkbox-group v-model="checkedSPU" @change="onOuterSelectionChange">
                        <el-checkbox :indeterminate="scope.row.commoditySPUId.isIndeterminate"
                            :label="scope.row.commoditySPUId">
                            <br>
                        </el-checkbox>
                    </el-checkbox-group>
                </template>
            </el-table-column>
            <el-table-column type="expand" width="40" align="center">
                <template #default="props">
                    <div style="margin-top: -10px;">
                        <el-table :data="props.row.detailList" border>
                            <el-table-column label="选择" width="55" align="center">
                                <template #default="scope">
                                    <el-checkbox-group v-model="checkedSKU" @change="onInnerSelectionChange">
                                        <el-checkbox :label="scope.row.commoditySKUId">
                                        </el-checkbox>
                                    </el-checkbox-group>
                                </template>
                            </el-table-column>
                            <el-table-column label="规格编码" prop="commoditySKUCode" />
                            <el-table-column label="规格名称" prop="commoditySKUName" />
                            <el-table-column label="商品单位" prop="unit" />
                            <el-table-column label="商品重量" prop="weight" />
                            <el-table-column label="商品长度" prop="length" />
                            <el-table-column label="商品宽度" prop="width" />
                            <el-table-column label="商品高度" prop="height" />
                            <el-table-column label="商品体积" prop="volume" />
                            <el-table-column label="商品成本" prop="cost" />
                            <el-table-column label="商品价格" prop="price" />
                            <el-table-column label="操作" />
                        </el-table>
                    </div>
                </template>
            </el-table-column>
            <el-table-column label="商品编码" prop="commoditySPUCode" align="center" />
            <el-table-column label="商品名称" prop="commoditySPUName" align="center" />
            <el-table-column label="商品类别" prop="categoryName" align="center" />
            <el-table-column label="商品描述" prop="commoditySPUDescription" align="center" />
            <el-table-column label="商品条码" prop="barCode" align="center" />
            <el-table-column label="供应商名称" prop="supplierName" align="center" />
            <el-table-column label="品牌" prop="brand" align="center" />
            <el-table-column label="操作" align="center">
                <template #default="scope">
                    <div>
                        <el-button text icon="edit" title="编辑" @click.stop="handleUpdate(scope.row)">
                        </el-button>
                        <el-button text icon="delete" title="删除" @click.stop="handleDelete(scope.row)">
                        </el-button>
                    </div>
                </template>
            </el-table-column>
        </el-table>

        <!-- 分页栏 -->
        <pagination v-show="total > 0" :total="total" v-model:page="queryParams.pageNum"
            v-model:limit="queryParams.pageSize" @pagination="getList" />

        <!-- 新增或编辑对话框 -->
        <el-dialog :title="title" v-model="open" width="500px" :show-close="false" :draggable="true">
            <el-form :model="form" :rules="rules" ref="SPURef" label-width="100px">

            </el-form>

            <template #footer>
                <el-button text @click="cancel">取消</el-button>
                <el-button type="primary" @click="submitForm">提交</el-button>
            </template>
        </el-dialog>
    </div>
</template>

<script setup>
const checkedSPU = ref([])
const checkedSKU = ref([])
const checkAll = ref(false)
const isIndeterminate = ref(false)
// 总条数
const total = ref(0);
// 展示搜索界面
const showSearch = ref(false);
// 加载...
const loading = ref(true);
// 商品列表
const commoditySPUList = ref([]);
// 展示对话框
const open = ref(false);
// 对话框标题
const title = ref("");
// 单位信息
const lengthUnit = ["mm", "cm", "dm", "m"]
const weightUnit = ["cm³", "dm³", "m³"]
const volumeUnit = ["mg", "g", "kg"]
// 时间范围
const dateRange = ref([]);
// 数据
const data = reactive({
    form: {},
    queryParams: {
        pageNum: 1,
        pageSize: 10,
        commoditySPUName: undefined,
        commoditySPUCode: undefined,
        categoryName: undefined
    },
    rules: {}
})
// 表单、搜索参数、规则
const { form, queryParams, rules } = toRefs(data)
const { proxy } = getCurrentInstance()
/**
 * 查询商品列表
 */
function getList() {
    loading.value = false
    commoditySPUList.value = [
        {
            commoditySPUId: 1,
            commoditySPUCode: "2313",
            commoditySPUName: "weqe",
            categoryName: "步进电机",
            commoditySPUDescription: "eqweq",
            barCode: "ewq",
            supplierName: "大大大",
            brand: "eqwewq",
            lengthUnit: 1,
            weightUnit: 1,
            volumeUnit: 1,
            detailList: [
                {
                    commoditySKUId: 1,
                    commoditySKUCode: "2313",
                    commoditySKUName: "weqe",
                    unit: "个",
                    weight: "2",
                    length: "30",
                    width: "20",
                    height: "25",
                    volume: "0",
                    cost: "100",
                    price: "125"
                },
                {
                    commoditySKUId: 2,
                    commoditySKUCode: "2313",
                    commoditySKUName: "weqe",
                    unit: "个",
                    weight: "2",
                    length: "30",
                    width: "20",
                    height: "25",
                    volume: "0",
                    cost: "100",
                    price: "125"
                }
            ]
        },
        {
            commoditySPUId: 2,
            commoditySPUCode: "2313",
            commoditySPUName: "weqe",
            categoryName: "步进电机",
            commoditySPUDescription: "eqweq",
            barCode: "ewq",
            supplierName: "大大大",
            brand: "eqwewq",
            lengthUnit: 1,
            weightUnit: 1,
            volumeUnit: 1,
            detailList: [
                {
                    commoditySKUId: 3,
                    commoditySKUCode: "2313",
                    commoditySKUName: "weqe",
                    unit: "个",
                    weight: "2",
                    length: "30",
                    width: "20",
                    height: "25",
                    volume: "0",
                    cost: "100",
                    price: "125"
                }
            ]
        }
    ]
}

/**
 * 搜索商品数据
 */
function handleQuery() {

}

/**
 * 重置搜索
 */
function resetQuery() {

}

/**
 * 新增按钮操作
 */
function handleAdd() {
    open.value = true
    title.value = "新增商品"
}

/**
 * 导出按钮操作
 */
function handleExport() {

}

/**
 * 生成二维码按钮操作
 */
function handleQrCode() {

}

/**
 * 生成条形码按钮操作
 */
function handleBarCode() {

}

/**
 * 表格多选操作
 */
function onTableSelectionChange(val) {
    let list = proxy.$refs.tableRef.data;
    let arr = []
    list.forEach(element => {
        let id = element.commoditySPUId
        arr.push(id)
    });
    checkedSPU.value = val ? arr : []
    isIndeterminate.value = false
}

/**
 * 外表格多选操作
 * @param {商品编号} id 
 */
function onOuterSelectionChange(ids) {
    let count = proxy.$refs.tableRef.data.length;
    checkAll.value = ids.length === count
    isIndeterminate.value = ids.length > 0 && ids.length < count
}

/**
 * 内表格多选操作
 * @param {商品编号} id 
 */
function onInnerSelectionChange(id) {
    console.log(id);
}

/**
 * 修改按钮操作
 * @param {行数据} row
 */
function handleUpdate(row) {
    open.value = true
    title.value = "编辑商品"
}

/**
 * 删除按钮操作
 * @param {行数据} row
 */
function handleDelete(row) {

}

/**
 * 新增对话框取消按钮
 */
function cancel() {
    open.value = false
}

/**
 * 新增对话框提交表单按钮
 */
function submitForm() {

}

getList()

</script>

<style lang="scss" scoped></style>