<template>
    <div class="app-container">
        <!-- 搜索功能 -->
        <el-form :model="queryParams" ref="queryRef" v-show="showSearch" :inline="true">
            <el-form-item label="仓库名称" prop="warehouseName">
                <el-input v-model="queryParams.warehouseName" placeholder="请输入仓库名称" clearable style="width: 160px" />
            </el-form-item>
            <el-form-item label="库区名称" prop="regionName">
                <el-input v-model="queryParams.regionName" placeholder="请输入库区名称" clearable style="width: 160px" />
            </el-form-item>
            <el-form-item label="库位编码" prop="locationCode">
                <el-input v-model="queryParams.locationCode" placeholder="请输入库位编码" clearable style="width: 160px" />
            </el-form-item>
            <el-form-item label="创建时间">
                <el-date-picker v-model="dateRange" style="width: 300px" type="daterange" range-separator="-"
                    start-placeholder="开始日期" end-placeholder="结束日期"></el-date-picker>
            </el-form-item>
            <el-form-item>
                <el-button type="primary" icon="search" @click="handleQuery">搜索</el-button>
                <el-button icon="refresh" @click="resetQuery">重置</el-button>
            </el-form-item>
        </el-form>

        <!-- 工具栏 -->
        <el-row :gutter="10">
            <el-col :span="1.5">
                <el-button type="primary" plain icon="plus" @click="handleAdd">新增</el-button>
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
            <right-toolbar v-model:showSearch="showSearch" @queryTable="getList" :columns="columns"></right-toolbar>
        </el-row>

        <!-- 表格 -->
        <el-table v-loading="loading" :data="locationList" highlight-current-row @selection-change="handleSelectionChange">
            <el-table-column type="selection" width="55" align="center" />
            <el-table-column label="仓库名称" prop="warehouseName" v-if="columns.showColumn('warehouseName')" />
            <el-table-column label="库区名称" prop="regionName" v-if="columns.showColumn('regionName')" />
            <el-table-column label="库区类型" prop="regionProperty" v-if="columns.showColumn('regionProperty')">
                <template #default="scope">
                    <span>{{ regionPropertyOptions[scope.row.regionProperty] }}</span>
                </template>
            </el-table-column>
            <el-table-column label="库位编码" prop="locationCode" v-if="columns.showColumn('locationCode')" />
            <el-table-column label="库位长(m)" prop="locationLength" v-if="columns.showColumn('locationLength')" />
            <el-table-column label="库位宽(m)" prop="locationWidth" v-if="columns.showColumn('locationWidth')" />
            <el-table-column label="库位高(m)" prop="locationHeight" v-if="columns.showColumn('locationHeight')" />
            <el-table-column label="库位容积(m³)" prop="locationVolume" v-if="columns.showColumn('locationVolume')" />
            <el-table-column label="库位承重(kg)" prop="locationLoad" v-if="columns.showColumn('locationLoad')" />
            <el-table-column label="巷道号" prop="roadwayNumber" v-if="columns.showColumn('roadwayNumber')" />
            <el-table-column label="货架号" prop="shelfNumber" v-if="columns.showColumn('shelfNumber')" />
            <el-table-column label="层号" prop="layerNumber" v-if="columns.showColumn('layerNumber')" />
            <el-table-column label="位号" prop="tagNumber" v-if="columns.showColumn('tagNumber')" />
            <el-table-column label="创建人" prop="createBy" v-if="columns.showColumn('createBy')" />
            <el-table-column label="创建时间" prop="createTime" v-if="columns.showColumn('createTime')" />
            <el-table-column label="是否有效" prop="isValid" v-if="columns.showColumn('isValid')">
                <template #default="scope">
                    <el-tag v-if="scope.row.isValid === 1" type="success">是</el-tag>
                    <el-tag v-else type="danger">否</el-tag>
                </template>
            </el-table-column>

            <!-- 操作行 -->
            <el-table-column label="操作" align="center" width="120" v-if="columns.showColumn('operate')">
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

        <!-- 对话框 -->
        <el-dialog :title="title" v-model="open" width="400px" :show-close="false" :draggable="true"
            style="overflow: hidden">
            <div style="overflow-y: auto; height: 350px">
                <el-form :model="form" :rules="rules" ref="locationRef" label-width="100px">
                    <el-form-item label="仓库名称" prop="warehouseId">
                        <el-select v-model="form.warehouseId" placeholder="请选择仓库" @change="warehouseChangeSelect"
                            style="width: 100%;">
                            <el-option v-for="item in warehouseOptions" :key="item.warehouseId" :label="item.warehouseName"
                                :value="item.warehouseId" />
                        </el-select>
                    </el-form-item>
                    <el-form-item label="库区名称" prop="regionId">
                        <el-select v-model="form.regionId" placeholder="请选择库区" @change="regionChangeSelect"
                            style="width: 100%;">
                            <el-option v-for="item in regionOptions" :key="item.regionId" :label="item.regionName"
                                :value="item.regionId" />
                        </el-select>
                    </el-form-item>
                    <el-form-item label="库区类型" prop="regionProperty">
                        <el-select v-model="form.regionProperty" disabled placeholder="请选择库区类型" style="width: 100%;">
                            <el-option v-for="(item, index) in regionPropertyOptions" :key="index" :label="item"
                                :value="index" />
                        </el-select>
                    </el-form-item>
                    <el-form-item label="库位编码" prop="locationCode">
                        <el-input v-model="form.locationCode" placeholder="请输入库位编码" clearable />
                    </el-form-item>
                    <el-form-item label="库位长(m)" prop="locationLength">
                        <el-input v-model="form.locationLength" placeholder="请输入库位长(m)" clearable />
                    </el-form-item>
                    <el-form-item label="库位宽(m)" prop="locationWidth">
                        <el-input v-model="form.locationWidth" placeholder="请输入库位宽(m)" clearable />
                    </el-form-item>
                    <el-form-item label="库位高(m)" prop="locationHeight">
                        <el-input v-model="form.locationHeight" placeholder="请输入库位高(m)" clearable />
                    </el-form-item>
                    <el-form-item label="库位容积(m³)" prop="locationVolume">
                        <el-input v-model="form.locationVolume" placeholder="请输入库位容积(m³)" clearable />
                    </el-form-item>
                    <el-form-item label="库位承重(kg)" prop="locationLoad">
                        <el-input v-model="form.locationLoad" placeholder="请输入库位承重(kg)" clearable />
                    </el-form-item>
                    <el-form-item label="巷道号" prop="roadwayNumber">
                        <el-input v-model="form.roadwayNumber" placeholder="请输入巷道号" clearable />
                    </el-form-item>
                    <el-form-item label="货架号" prop="shelfNumber">
                        <el-input v-model="form.shelfNumber" placeholder="请输入货架号" clearable />
                    </el-form-item>
                    <el-form-item label="层号" prop="layerNumber">
                        <el-input v-model="form.layerNumber" placeholder="请输入层号" clearable />
                    </el-form-item>
                    <el-form-item label="位号" prop="tagNumber">
                        <el-input v-model="form.tagNumber" placeholder="请输入位号" clearable />
                    </el-form-item>
                    <el-form-item label="是否有效" prop="isValid">
                        <el-switch v-model="form.isValid" :active-value="1" :inactive-value="0" />
                    </el-form-item>
                </el-form>
            </div>

            <template #footer>
                <el-button text @click="cancel">取消</el-button>
                <el-button type="primary" @click="submitForm">提交</el-button>
            </template>
        </el-dialog>

        <zQrBarDialog v-model:visible="codeOpen" :qrCodeValues="qrcodeData" :barCodeValues="barcodeData"
            :codeMode="codeMode">
            <template #default="scope">
                <span style="display: block;">仓库名称: {{ scope.item.warehouseName }}</span>
                <span style="display: block;">库区名称: {{ scope.item.regionName }}</span>
                <span style="display: block;">库区类型: {{ scope.item.regionProperty }}</span>
                <span style="display: block;">库位编码: {{ scope.item.locationCode }}</span>
            </template>
        </zQrBarDialog>

    </div>
</template>

<script setup>
import { getAllInfo, getRegionOptions, getWarehouseOptions, addInfo, editInfo, exportAllInfo, getInfo, deleteInfo } from '@/api/warehousManagement/location'

// 总条数
const total = ref(0)
// 展示搜索界面
const showSearch = ref(false)
// 加载...
const loading = ref(true)
// 供应商列表
const locationList = ref([])
// 库区类型
const regionPropertyOptions = ["拣货区", "备货区", "收货区", "退货区", "次品区", "存货区"]
//仓库选项列表
const warehouseOptions = ref([])
//库区选项列表
const regionOptions = ref([])
// 展示对话框
const open = ref(false)
// 对话框标题
const title = ref('')
// 二维码或条形码数据
const codeData = ref([])
const qrcodeData = ref([])
const barcodeData = ref([])
const codeOpen = ref(false)
const codeMode = ref("")
// 时间范围
const dateRange = ref([])
// 数据
const data = reactive({
    form: {
        warehouseId: undefined,
        regionId: undefined,
        regionProperty: undefined,
        locationCode: undefined,
        locationLength: 0,
        locationWidth: 0,
        locationHeight: 0,
        locationVolume: 0,
        locationLoad: 0,
        roadwayNumber: undefined,
        shelfNumber: undefined,
        layerNumber: undefined,
        tagNumber: undefined,
        isValid: 1
    },
    queryParams: {
        pageNum: 1,
        pageSize: 10,
        warehouseName: undefined,
        regionName: undefined,
        locationCode: undefined
    },
    rules: {
        warehouseId: [{ required: true, message: '仓库名称不能为空', trigger: 'blur' }],
        regionId: [{ required: true, message: '库区名称不能为空', trigger: 'blur' }],
        locationCode: [{ required: true, message: '库区编码只能为字母加数字组合', trigger: 'blur', pattern: /^[a-zA-Z0-9]+$/ }],
    }
})
// 列显隐信息
const columns = ref([
    { key: 0, label: `仓库名称`, visible: true, prop: 'warehouseName' },
    { key: 1, label: `库区名称`, visible: true, prop: 'regionName' },
    { key: 2, label: `库区类型`, visible: true, prop: 'regionProperty' },
    { key: 3, label: `库位编码`, visible: true, prop: 'locationCode' },
    { key: 4, label: `库位长(m)`, visible: true, prop: 'locationLength' },
    { key: 5, label: `库位宽(m)`, visible: true, prop: 'locationWidth' },
    { key: 6, label: `库位高(m)`, visible: true, prop: 'locationHeight' },
    { key: 7, label: `库位容积(m³)`, visible: true, prop: 'locationVolume' },
    { key: 8, label: `库位承重(kg)`, visible: true, prop: 'locationLoad' },
    { key: 9, label: `巷道号`, visible: true, prop: 'roadwayNumber' },
    { key: 10, label: `货架号`, visible: true, prop: 'shelfNumber' },
    { key: 11, label: `层号`, visible: true, prop: 'layerNumber' },
    { key: 12, label: `位号`, visible: true, prop: 'tagNumber' },
    { key: 13, label: `创建人`, visible: false, prop: 'createBy' },
    { key: 14, label: `创建时间`, visible: false, prop: 'createTime' },
    { key: 15, label: `是否有效`, visible: true, prop: 'isValid' },
    { key: 16, label: `操作`, visible: true, prop: 'operate' }
])
// 表单、搜索参数、规则
const { form, queryParams, rules } = toRefs(data)
// 当时实例
const { proxy } = getCurrentInstance()

/**
 * 重置操作表单
 */
function reset() {
    form.value = {
        warehouseId: undefined,
        regionId: undefined,
        regionProperty: undefined,
        locationCode: undefined,
        locationLength: 0,
        locationWidth: 0,
        locationHeight: 0,
        locationVolume: 0,
        locationLoad: 0,
        roadwayNumber: undefined,
        shelfNumber: undefined,
        layerNumber: undefined,
        tagNumber: undefined,
        isValid: 1
    }
    proxy.resetForm('locationRef')
}

/**
 * 获取列表
 */
function getList() {
    loading.value = true
    let params = proxy.addDateRange(queryParams.value, dateRange.value)
    getAllInfo(params).then(res => {
        loading.value = false
        locationList.value = res.data.result
        total.value = res.data.totalNum
    })
    getWarehouseOptions().then(res => {
        warehouseOptions.value = res.data
    })
}

/**
 * 搜索按钮操作
 */
function handleQuery() {
    queryParams.pageNum = 1
    getList()
}

/**
 * 重置按钮操作
 */
function resetQuery() {
    dateRange.value = []
    proxy.resetForm('queryRef')
    handleQuery()
}

/**
 * 新增按钮操作
 */
function handleAdd() {
    reset()
    open.value = true
    title.value = "新增库位信息"
}

/**
 * 导出按钮操作 
 */
function handleExport() {
    proxy
        .$modal
        .confirm('是否确认导出所有库区信息数据项?', '警告', {
            confirmButtonText: '确定',
            cancelButtonText: '取消',
            type: 'warning'
        })
        .then(async () => {
            await exportAllInfo()
        })
}

/**
 * 生成二维码按钮操作
 */
function handleQrCode() {
    qrcodeData.value = []
    qrcodeData.value = [...codeData.value]
    codeOpen.value = true
    codeMode.value = "qrcode"
}

/**
 * 生成条形码按钮操作
 */
function handleBarCode() {
    barcodeData.value = []
    barcodeData.value = codeData.value.map(item => item.locationCode)
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
        codeData.value.push({
            warehouseName: element.warehouseName,
            regionName: element.regionName,
            regionProperty: regionPropertyOptions[element.regionProperty],
            locationCode: element.locationCode
        })
    });
}

/**
 * 选中值发生变化时触发
 * @param {选中值} value 
 */
function warehouseChangeSelect(value) {
    regionOptions.value = []
    form.value.regionId = undefined
    form.value.regionProperty = undefined
    getRegionOptions(value).then(res => {
        regionOptions.value = res.data;
    })
}

/**
 * 选中值发生变化时触发
 * @param {选中值} value 
 */
function regionChangeSelect(value) {
    regionOptions.value.forEach(element => {
        if (element.regionId === value) {
            form.value.regionProperty = element.regionProperty
        }
    })
}

/**
 * 提交按钮
 */
function submitForm() {
    proxy.$refs['locationRef'].validate((valid) => {
        if (valid) {
            if (form.value.locationId != undefined) {
                editInfo(form.value.locationId, form.value).then(res => {
                    proxy.$modal.msgSuccess('修改成功')
                    open.value = false
                    getList()
                })
            } else {
                addInfo(form.value).then(res => {
                    proxy.$modal.msgSuccess('新增成功')
                    open.value = false
                    getList()
                })
            }
        }
    })
}

/**
 * 取消按钮
 */
function cancel() {
    open.value = false
    reset()
}

/**
 * 修改按钮操作 
 * @param {行数据} row 
 */
function handleUpdate(row) {
    reset()
    const locationId = row.locationId
    getInfo(locationId).then(response => {
        form.value = response.data
        getRegionOptions(form.value.warehouseId).then(res => {
            regionOptions.value = res.data;
        })
        open.value = true
        title.value = '修改库区信息'
    })
    open.value = true
}

/**
 * 删除按钮操作
 * @param {行数据} row 
 */
function handleDelete(row) {
    const locationId = row.locationId
    proxy
        .$modal
        .confirm('是否确认删除"' + row.locationCode + '"的数据项？')
        .then(() => {
            return deleteInfo(locationId)
        })
        .then(() => {
            getList()
            proxy.$modal.msgSuccess('删除成功')
        })
        .catch(() => { })
}

getList()
</script>

<style></style>