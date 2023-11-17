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
            <el-form-item label="商品类别" prop="categoryId">
                <!-- <el-input v-model="queryParams.categoryName" placeholder="请输入商品类别" clearable /> -->
                <el-cascader v-model="queryParams.categoryId" :options="categoryOptions"
                    :props="{ checkStrictly: true, value: 'categoryId', label: 'categoryName', children: 'children', emitPath: false }" />
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
        <el-table v-loading="loading" :data="commodityList" highlight-current-row border
            @selection-change="handleSelectionChange">
            <el-table-column type="selection" width="55" align="center" />
            <el-table-column type="expand" width="40" align="center">
                <template #default="props">
                    <div style="margin-top: -10px;">
                        <el-table :data="props.row.detailList" border>
                            <el-table-column label="规格编码" prop="commoditySKUCode" />
                            <el-table-column label="规格名称" prop="commoditySKUName" />
                            <el-table-column label="商品单位" prop="unit" />
                            <el-table-column label="商品重量" prop="weight">
                                <template #default="scope">
                                    <span>{{ scope.row.weight }} {{ weightUnit[props.row.weightUnit] }}</span>
                                </template>
                            </el-table-column>
                            <el-table-column label="商品长度" prop="length">
                                <template #default="scope">
                                    <span>{{ scope.row.length }} {{ lengthUnit[props.row.lengthUnit] }}</span>
                                </template>
                            </el-table-column>
                            <el-table-column label="商品宽度" prop="width">
                                <template #default="scope">
                                    <span>{{ scope.row.width }} {{ lengthUnit[props.row.lengthUnit] }}</span>
                                </template>
                            </el-table-column>
                            <el-table-column label="商品高度" prop="height">
                                <template #default="scope">
                                    <span>{{ scope.row.height }} {{ lengthUnit[props.row.lengthUnit] }}</span>
                                </template>
                            </el-table-column>
                            <el-table-column label="商品体积" prop="volume">
                                <template #default="scope">
                                    <span>{{ scope.row.volume }} {{ volumeUnit[props.row.volumeUnit] }}</span>
                                </template>
                            </el-table-column>
                            <el-table-column label="商品成本" prop="cost" />
                            <el-table-column label="商品价格" prop="price" />
                            <el-table-column label="操作" align="center">
                                <template #default="scope">
                                    <div>
                                        <el-button text icon="delete" title="删除" @click.stop="handleSKUDelete(scope.row)" />
                                    </div>
                                </template>
                            </el-table-column>
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
        <el-dialog :title="title" v-model="open" width="1000px" :show-close="false" :draggable="true">
            <div>
                <el-form :model="form" :rules="rules" ref="commodityRef" label-width="auto" class="dialog-container">
                    <div class="left-form">
                        <el-form-item label="商品编码" prop="commoditySPUCode">
                            <el-input v-model="form.commoditySPUCode" placeholder="请输入商品编码" clearable />
                        </el-form-item>
                        <el-form-item label="商品名称" prop="commoditySPUName">
                            <el-input v-model="form.commoditySPUName" placeholder="请输入商品名称" clearable />
                        </el-form-item>
                        <el-form-item label="商品类别" prop="categoryId">
                            <el-cascader v-model="form.categoryId" :options="categoryOptions"
                                :props="{ checkStrictly: true, value: 'categoryId', label: 'categoryName', children: 'children', emitPath: false }" />
                        </el-form-item>
                        <el-form-item label="商品描述" prop="commoditySPUDescription">
                            <el-input v-model="form.commoditySPUDescription" placeholder="请输入商品描述" clearable />
                        </el-form-item>
                        <el-form-item label="商品条形码" prop="barCode">
                            <el-input v-model="form.barCode" placeholder="请输入商品条形码" clearable />
                        </el-form-item>
                        <el-form-item label="供应商名称" prop="supplierId">
                            <el-select v-model="form.supplierId" placeholder="请选择供应商名称" clearable>
                                <el-option v-for="item in supplierOptions" :key="item.supplierId" :label="item.supplierName"
                                    :value="item.supplierId" />
                            </el-select>
                        </el-form-item>
                        <el-form-item label="品牌" prop="brand">
                            <el-input v-model="form.brand" placeholder="请输入品牌" clearable />
                        </el-form-item>
                        <el-form-item label="长度单位" prop="lengthUnit">
                            <el-select v-model="form.lengthUnit" placeholder="请输入长度单位" clearable>
                                <el-option key="0" label="mm" value="0" />
                                <el-option key="1" label="cm" value="1" />
                                <el-option key="2" label="dm" value="2" />
                                <el-option key="3" label="mm" value="3" />
                            </el-select>
                        </el-form-item>
                        <el-form-item label="体积单位" prop="volumeUnit">
                            <el-select v-model="form.volumeUnit" placeholder="请输入体积单位" clearable>
                                <el-option key="0" label="cm³" value="0" />
                                <el-option key="1" label="dm³" value="1" />
                                <el-option key="2" label="m³" value="2" />
                            </el-select>
                        </el-form-item>
                        <el-form-item label="重量单位" prop="weightUnit">
                            <el-select v-model="form.weightUnit" placeholder="请输入重量单位" clearable>
                                <el-option key="0" label="mg" value="0" />
                                <el-option key="1" label="g" value="1" />
                                <el-option key="2" label="kg" value="2" />
                            </el-select>
                        </el-form-item>
                    </div>
                    <div class="right-table">
                        <div style="padding-bottom: 3px;">
                            <el-button type="primary" plain icon="Plus" @click="handleDialogAdd" />
                            <el-button type="warning" plain icon="download" disabled />
                        </div>
                        <el-table :data="form.detailList" border :header-cell-style="{ textAlign: 'center' }"
                            @cell-dblclick="onCellDblclick" :row-class-name="onRowClassName"
                            :cell-class-name="onCellClassName">
                            <el-table-column prop="commoditySKUCode" width="100px">
                                <template #header>
                                    <span style="color: #f56c6c;">* </span>
                                    <span>规格编码</span>
                                </template>
                                <template #default="scope">
                                    <div
                                        v-if="tableRowEditIndex === scope.row.index && tableColumnEditIndex === scope.column.index">
                                        <el-form-item prop="commoditySKUCode" style="margin-bottom: 0px;">
                                            <el-input v-model="scope.row.commoditySKUCode"
                                                @blur="onInputBlur(event, scope.row.commoditySKUCode, '规格编码')" />
                                        </el-form-item>
                                    </div>
                                    <div v-else style="height: 23px;">
                                        {{ scope.row.commoditySKUCode }}
                                    </div>
                                </template>
                            </el-table-column>
                            <el-table-column prop="commoditySKUName" width="100px">
                                <template #header>
                                    <span style="color: #f56c6c;">* </span>
                                    <span>规格名称</span>
                                </template>
                                <template #default="scope">
                                    <div
                                        v-if="tableRowEditIndex === scope.row.index && tableColumnEditIndex === scope.column.index">
                                        <el-form-item prop="commoditySKUName" style="margin-bottom: 0px;">
                                            <el-input v-model="scope.row.commoditySKUName"
                                                @blur="onInputBlur(event, scope.row.commoditySKUName, '规格名称')" />
                                        </el-form-item>
                                    </div>
                                    <div v-else style="height: 23px;">
                                        {{ scope.row.commoditySKUName }}
                                    </div>
                                </template>
                            </el-table-column>
                            <el-table-column prop="unit" width="100px">
                                <template #header>
                                    <span style="color: #f56c6c;">* </span>
                                    <span>商品单位</span>
                                </template>
                                <template #default="scope">
                                    <div
                                        v-if="tableRowEditIndex === scope.row.index && tableColumnEditIndex === scope.column.index">
                                        <el-form-item prop="unit" style="margin-bottom: 0px;">
                                            <el-input v-model="scope.row.unit"
                                                @blur="onInputBlur(event, scope.row.unit, '商品单位')" />
                                        </el-form-item>
                                    </div>
                                    <div v-else style="height: 23px;">
                                        {{ scope.row.unit }}
                                    </div>
                                </template>
                            </el-table-column>
                            <el-table-column label="商品重量" prop="weight" width="100px">
                                <template #default="scope">
                                    <div
                                        v-if="tableRowEditIndex === scope.row.index && tableColumnEditIndex === scope.column.index">
                                        <el-form-item prop="weight" style="margin-bottom: 0px;">
                                            <el-input v-model="scope.row.weight" @blur="onInputBlur" />
                                        </el-form-item>
                                    </div>
                                    <div v-else style="height: 23px;">
                                        {{ scope.row.weight }}
                                    </div>
                                </template>
                            </el-table-column>
                            <el-table-column label="商品长度" prop="length" width="100px">
                                <template #default="scope">
                                    <div
                                        v-if="tableRowEditIndex === scope.row.index && tableColumnEditIndex === scope.column.index">
                                        <el-form-item prop="length" style="margin-bottom: 0px;">
                                            <el-input v-model="scope.row.length" @blur="onInputBlur" />
                                        </el-form-item>
                                    </div>
                                    <div v-else style="height: 23px;">
                                        {{ scope.row.length }}
                                    </div>
                                </template>
                            </el-table-column>
                            <el-table-column label="商品宽度" prop="width" width="100px">
                                <template #default="scope">
                                    <div
                                        v-if="tableRowEditIndex === scope.row.index && tableColumnEditIndex === scope.column.index">
                                        <el-form-item prop="width" style="margin-bottom: 0px;">
                                            <el-input v-model="scope.row.width" @blur="onInputBlur" />
                                        </el-form-item>
                                    </div>
                                    <div v-else style="height: 23px;">
                                        {{ scope.row.width }}
                                    </div>
                                </template>
                            </el-table-column>
                            <el-table-column label="商品高度" prop="height" width="100px">
                                <template #default="scope">
                                    <div
                                        v-if="tableRowEditIndex === scope.row.index && tableColumnEditIndex === scope.column.index">
                                        <el-form-item prop="height" style="margin-bottom: 0px;">
                                            <el-input v-model="scope.row.height" @blur="onInputBlur" />
                                        </el-form-item>
                                    </div>
                                    <div v-else style="height: 23px;">
                                        {{ scope.row.height }}
                                    </div>
                                </template>
                            </el-table-column>
                            <!-- <el-table-column label="商品体积" prop="volume" width="100px">
                                <template #default="scope">
                                    <div
                                        v-if="tableRowEditIndex === scope.row.index && tableColumnEditIndex === scope.column.index">
                                        <el-form-item prop="volume" style="margin-bottom: 0px;">
                                            <el-input v-model="scope.row.volume" @blur="onInputBlur" />
                                        </el-form-item>
                                    </div>
                                    <div v-else style="height: 23px;">
                                        {{ scope.row.volume }}
                                    </div>
                                </template>
                            </el-table-column> -->
                            <el-table-column label="商品成本" prop="cost" width="100px">
                                <template #default="scope">
                                    <div
                                        v-if="tableRowEditIndex === scope.row.index && tableColumnEditIndex === scope.column.index">
                                        <el-form-item prop="cost" style="margin-bottom: 0px;">
                                            <el-input v-model="scope.row.cost" @blur="onInputBlur" />
                                        </el-form-item>
                                    </div>
                                    <div v-else style="height: 23px;">
                                        {{ scope.row.cost }}
                                    </div>
                                </template>
                            </el-table-column>
                            <el-table-column label="商品价格" prop="price" width="100px">
                                <template #default="scope">
                                    <div
                                        v-if="tableRowEditIndex === scope.row.index && tableColumnEditIndex === scope.column.index">
                                        <el-form-item prop="price" style="margin-bottom: 0px;">
                                            <el-input v-model="scope.row.price" @blur="onInputBlur" />
                                        </el-form-item>
                                    </div>
                                    <div v-else style="height: 23px;">
                                        {{ scope.row.price }}
                                    </div>
                                </template>
                            </el-table-column>
                        </el-table>
                    </div>
                </el-form>
            </div>

            <template #footer>
                <el-button text @click="cancel">取消</el-button>
                <el-button type="primary" @click="submitForm">提交</el-button>
            </template>
        </el-dialog>

        <zQrBarDialog v-model:visible="codeOpen" :codeValues="codeData" :codeMode="codeMode">
            <template #default="scope">
                <span style="display: block;">商品名称: {{ scope.item.spuname }}</span>
                <span style="display: block;">商品编码: {{ scope.item.spucode }}</span>
                <span style="display: block;">规格名称: {{ scope.item.skuname }}</span>
                <span style="display: block;">规格编码: {{ scope.item.skucode }}</span>
            </template>
        </zQrBarDialog>
    </div>
</template>

<script setup>
import { getAll, getCommodityInfo, deleteById, addCommodityInfo, editCommodityInfo, getCategoryOptions, getSupplierOptions, exportCommodity, deleteSKUById } from "@/api/warehousManagement/commodity";
// 总条数
const total = ref(0);
// 展示搜索界面
const showSearch = ref(false);
// 加载...
const loading = ref(true);
// 商品列表
const commodityList = ref([]);
// 展示对话框
const open = ref(false);
// 对话框标题
const title = ref("");
// 单位信息
const lengthUnit = ["mm", "cm", "dm", "m"]
const weightUnit = ["mg", "g", "kg"]
const volumeUnit = ["cm³", "dm³", "m³"]
// 二维码或条形码数据
const codeData = ref([])
const codeOpen = ref(false)
const codeMode = ref("")
// 时间范围
const dateRange = ref([]);
// 数据
const data = reactive({
    form: {
        commoditySPUCode: undefined,
        commoditySPUName: undefined,
        categoryId: undefined,
        commoditySPUDescription: undefined,
        barCode: undefined,
        supplierId: undefined,
        brand: undefined,
        lengthUnit: undefined,
        weightUnit: undefined,
        volumeUnit: undefined,
        detailList: []
    },
    // 编辑的表格行
    tableRowEditIndex: undefined,
    // 编辑的表格列
    tableColumnEditIndex: undefined,
    queryParams: {
        pageNum: 1,
        pageSize: 10,
        commoditySPUName: undefined,
        commoditySPUCode: undefined,
        categoryId: undefined
    },
    rules: {
        commoditySPUCode: [{ required: true, message: '商品编码不能为空', trigger: 'blur' }],
        commoditySPUName: [{ required: true, message: '商品名称不能为空', trigger: 'blur' }],
        categoryId: [{ required: true, message: '商品类别不能为空', trigger: 'blur' }],
        supplierId: [{ required: true, message: '供应商不能为空', trigger: 'blur' }],
        // commoditySKUCode: [{ required: true, message: '规格编码不能为空', trigger: 'blur' }],
    },
    categoryOptions: [
        {
            value: 'guide',
            label: 'Guide',
        }
    ],
    supplierOptions: []
})
// 表单、搜索参数、规则
const { form, tableRowEditIndex, tableColumnEditIndex, queryParams, rules, categoryOptions, supplierOptions } = toRefs(data)
const { proxy } = getCurrentInstance()
/**
 * 查询商品列表
 */
function getList() {
    loading.value = false
    let params = proxy.addDateRange(queryParams.value, dateRange.value);
    getAll(params).then(res => {
        loading.value = false
        commodityList.value = res.data.result
        total.value = res.data.totalNum
    })
    getSupplierOptions().then(res => {
        supplierOptions.value = res.data
    })
    getCategoryOptions().then(res => {
        categoryOptions.value = res.data
    })
}

/**
 * 重置操作表单
 */
function reset() {
    form.value = {
        commoditySPUCode: undefined,
        commoditySPUName: undefined,
        categoryName: undefined,
        commoditySPUDescription: undefined,
        barCode: undefined,
        supplierName: undefined,
        brand: undefined,
        lengthUnit: undefined,
        weightUnit: undefined,
        volumeUnit: undefined,
        detailList: []
    }
    proxy.resetForm('commodityRef')
}

/**
 * 搜索商品数据
 */
function handleQuery() {
    queryParams.pageNum = 1
    getList()
}

/**
 * 重置搜索
 */
function resetQuery() {
    proxy.resetForm('queryRef')
    handleQuery()
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
    proxy.$modal
        .confirm('是否确认导出所有商品数据项?', '警告', {
            confirmButtonText: '确定',
            cancelButtonText: '取消',
            type: 'warning'
        })
        .then(async () => {
            await exportCommodity()
        })
}

/**
 * 生成二维码按钮操作
 */
function handleQrCode() {
    codeOpen.value = true
    codeMode.value = "qrcode"
}

/**
 * 生成条形码按钮操作
 */
function handleBarCode() {
    codeOpen.value = true
    codeMode.value = "barcode"
}

/**
 * 选中数据操作
 * @param {选中的数据} selection 
 */
function handleSelectionChange(selection) {
    codeData.value = []
    selection.forEach(element => {
        element.detailList.forEach(childElement => {
            codeData.value.push({
                spuname: element.commoditySPUName,
                spucode: element.commoditySPUCode,
                skuname: childElement.commoditySKUName,
                skucode: childElement.commoditySKUCode
            })
        });
    });
}

/**
 * 删除商品sku信息
 * @param {行数据} row 
 */
function handleSKUDelete(row) {
    const commoditySKUId = row.commoditySKUId
    proxy.$modal
        .confirm('是否确认删除"' + row.commoditySKUName + '"的数据项？')
        .then(function () {
            return deleteSKUById(commoditySKUId)
        })
        .then(() => {
            getList()
            proxy.$modal.msgSuccess('删除成功')
        })
}

/**
 * 修改按钮操作
 * @param {行数据} row
 */
function handleUpdate(row) {
    open.value = true
    title.value = "编辑商品"
    getCommodityInfo(row.commoditySPUId).then(res => {
        console.log(res.data);
        form.value = res.data
    })
}

/**
 * 删除按钮操作
 * @param {行数据} row
 */
function handleDelete(row) {
    const commoditySPUId = row.commoditySPUId
    proxy.$modal
        .confirm('是否确认删除"' + row.commoditySPUName + '"的数据项？')
        .then(function () {
            return deleteById(commoditySPUId)
        })
        .then(() => {
            getList()
            proxy.$modal.msgSuccess('删除成功')
        })
}

/**
 * 对话框内新增按钮操作
 */
function handleDialogAdd() {
    form.value.detailList.push({})
}

/**
 * 单元格被双击击时会触发该事件
 * @param {行数据} row 
 * @param {列数据} column 
 * @param {单元数据} cell 
 * @param {事件} event 
 */
function onCellDblclick(row, column, cell, event) {
    tableRowEditIndex.value = row.index;
    tableColumnEditIndex.value = column.index;
}

/**
 * 把每一行的索引放进row
 * @param {行数据，行引索} param
 */
function onRowClassName({ row, rowIndex }) {
    row.index = rowIndex;
}

/**
 * 把每一列的索引放进column
 * @param {列数据，列引索} param0 
 */
function onCellClassName({ row, column, rowIndex, columnIndex }) {
    column.index = columnIndex;
}

/**
 * 输入框失去焦点时触发
 * @param {焦点事件} FocusEvent 
 */
function onInputBlur(FocusEvent, value, title) {
    tableRowEditIndex.value = undefined;
    tableColumnEditIndex.value = undefined;
    if (value === undefined && title !== undefined) {
        proxy.$modal.msgError(`请先填写完整${title}!`)
    }
}

/**
 * 新增对话框取消按钮
 */
function cancel() {
    reset()
    open.value = false
}

/**
 * 新增对话框提交表单按钮
 */
function submitForm() {
    proxy.$refs['commodityRef'].validate((valid) => {
        if (valid) {
            if (form.value.detailList.length === 0) {
                proxy.$modal.msgError("规格数据不能为空！")
                return
            }
            if (form.value.commoditySPUId != undefined) {
                editCommodityInfo(form.value.commoditySPUId, form.value).then(res => {
                    proxy.$modal.msgSuccess('修改成功')
                    open.value = false
                    getList()
                })
            } else {
                addCommodityInfo(form.value).then(res => {
                    proxy.$modal.msgSuccess('新增成功')
                    open.value = false
                    getList()
                })
            }
        }
    })
}

getList()

</script>

<style lang="scss" scoped>
.dialog-container {
    display: flex;
    height: 450px;
    margin: -20px 0;
}

.left-form {
    width: 30%;
    padding: 5px 5px 0 0;
    overflow-y: auto;
}

.right-table {
    width: 70%;
    padding: 0 0 0 5px;
    overflow-y: auto;
}
</style>