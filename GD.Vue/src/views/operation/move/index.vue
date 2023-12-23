<template>
    <div class="app-container">
        <!-- 搜索功能 -->
        <el-form :model="queryParams" ref="queryRef" v-show="showSearch" :inline="true">
            <el-form-item label="作业单号" prop="jobCode">
                <el-input v-model="queryParams.jobCode" placeholder="请输入作业单号" clearable style="width: 160px" />
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
            <right-toolbar v-model:showSearch="showSearch" @queryTable="getList" :columns="columns"></right-toolbar>
        </el-row>

        <!-- 表格 -->
        <el-table v-loading="loading" :data="moveList" highlight-current-row :stripe="true">
            <el-table-column label="作业单号" prop="jobCode" />
            <el-table-column label="作业状态" prop="moveStatus">
                <template #default="scope">
                    <el-tag type="" effect="dark" v-if="scope.row.moveStatus === 0">未调整</el-tag>
                    <el-tag color="#626aef" effect="dark" v-else>已调整</el-tag>
                </template>
            </el-table-column>
            <el-table-column label="商品编码" prop="spuCode" />
            <el-table-column label="商品名称" prop="spuName" />
            <el-table-column label="规格编码" prop="skuCode" />
            <el-table-column label="规格名称" prop="skuName" />
            <el-table-column label="数量" prop="qty" />
            <el-table-column label="来源仓库" prop="origWarehousName" />
            <el-table-column label="来源库位" prop="origLocationCode" />
            <el-table-column label="目标仓库" prop="destWarehousName" />
            <el-table-column label="目标库位" prop="destLocationCode" />
            <el-table-column label="操作人" prop="handler" />
            <el-table-column label="操作时间" prop="handlerTime" />
            <el-table-column label="创建人" prop="createBy" v-if="columns.showColumn('createBy')" />
            <el-table-column label="创建时间" prop="createTime" v-if="columns.showColumn('createTime')" />

            <!-- 操作行 -->
            <el-table-column label="操作" align="center" width="120" v-if="columns.showColumn('operate')">
                <template #default="scope">
                    <div>
                        <!-- <el-button text icon="view" title="查看" @click.stop="handleDetail(scope.row)" /> -->
                        <el-button text icon="check" title="确认加工" :disabled="scope.row.moveStatus === 0 ? false : true"
                            @click.stop="handleMove(scope.row)" />
                        <el-button text icon="delete" title="删除" :disabled="scope.row.moveStatus === 0 ? false : true"
                            @click.stop="handleDelete(scope.row)" />
                    </div>
                </template>
            </el-table-column>
        </el-table>

        <!-- 分页栏 -->
        <pagination v-show="total > 0" :total="total" v-model:page="queryParams.pageNum"
            v-model:limit="queryParams.pageSize" @pagination="getList" />

        <!-- 对话框 -->
        <el-dialog :title="title" v-model="open" :show-close="false" width="25%">
            <el-form :model="form" :rules="rules" ref="moveRef" label-width="80px">
                <el-form-item label="商品编码" prop="spuCode">
                    <el-input v-model="form.spuCode" placeholder="请输入商品编码" :readonly="true" @click.native="spuCodeClick" />
                </el-form-item>
                <el-form-item label="商品名称" prop="spuName">
                    <el-input v-model="form.spuName" placeholder="请输入商品名称" disabled style="pointer-events: none;" />
                </el-form-item>
                <el-form-item label="规格编码" prop="skuCode">
                    <el-input v-model="form.skuCode" placeholder="请输入规格编码" disabled style="pointer-events: none;" />
                </el-form-item>
                <el-form-item label="来源仓库" prop="origWarehousName">
                    <el-input v-model="form.origWarehousName" placeholder="请输入来源仓库" disabled
                        style="pointer-events: none;" />
                </el-form-item>
                <el-form-item label="来源库位" prop="origLocationCode">
                    <el-input v-model="form.origLocationCode" placeholder="请输入来源库位" disabled
                        style="pointer-events: none;" />
                </el-form-item>
                <el-form-item label="目标仓库" prop="destWarehousName">
                    <el-input v-model="form.destWarehousName" placeholder="请输入目标仓库" disabled
                        style="pointer-events: none;" />
                </el-form-item>
                <el-form-item label="目标库位" prop="destLocationCode">
                    <el-input v-model="form.destLocationCode" placeholder="请输入目标库位" :readonly="true"
                        @click.native="destLocationCodeClick" />
                </el-form-item>
                <el-form-item label="数量" prop="qty">
                    <el-input v-model="form.qty" placeholder="请输入数量" />
                </el-form-item>
            </el-form>

            <template #footer>
                <el-button text @click="cancel">取消</el-button>
                <el-button type="primary" @click="submitForm">提交</el-button>
            </template>
        </el-dialog>

        <!-- 库存选择 -->
        <z-StockSelectDialog v-model:visible="stockSelectOpen" @dialogData="dialogStockData" />

        <!-- 库位选择 -->
        <z-LocationSelectDialog v-model:visible="locationSelectOpen" @dialogData="dialogLocationData" />

    </div>
</template>

<script setup>
import { getAllInfo, getInfo, addInfo, deleteInfo, confirmMove, exportAllInfo } from "@/api/operation/move";

// 总条数
const total = ref(0)
// 展示搜索界面
const showSearch = ref(false)
// 加载...
const loading = ref(true)
// 仓库移动列表
const moveList = ref([])
// 展示对话框
const open = ref(false)
// 展示库存选择对话框
const stockSelectOpen = ref(false)
// 展示库位选择对话框
const locationSelectOpen = ref(false)
// 对话框标题
const title = ref('')
// 时间范围
const dateRange = ref([])
// 数据
const data = reactive({
    form: {
        skuId: undefined,
        spuCode: undefined,
        spuName: undefined,
        skuCode: undefined,
        origLocationId: undefined,
        origWarehousName: undefined,
        origLocationCode: undefined,
        destLocationId: undefined,
        destWarehousName: undefined,
        destLocationCode: undefined,
        qty: undefined,
        ownerId: undefined
    },
    queryParams: {
        pageNum: 1,
        pageSize: 10,
        jobCode: undefined
    },
    rules: {
        spuCode: [{ required: true, message: '商品编码不能为空', trigger: 'blur' }],
        spuName: [{ required: true, message: '商品名称不能为空', trigger: 'blur' }],
        skuCode: [{ required: true, message: '规格编码不能为空', trigger: 'blur' }],
        origWarehousName: [{ required: true, message: '来源仓库不能为空', trigger: 'blur' }],
        origLocationCode: [{ required: true, message: '来源库位不能为空', trigger: 'blur' }],
        destWarehousName: [{ required: true, message: '目标仓库不能为空', trigger: 'blur' }],
        destLocationCode: [{ required: true, message: '目标库位不能为空', trigger: 'blur' }],
        qty: [{ required: true, message: '数量不能为空', trigger: 'blur', pattern: /^[0-9]+$/ },
        { validator: validQty, trigger: "blur" }],
    },
    curAvailableQty: 0,
})
// 列显隐信息
const columns = ref([
    { key: 0, label: `创建人`, visible: false, prop: 'createBy' },
    { key: 1, label: `创建时间`, visible: false, prop: 'createTime' },
    { key: 2, label: `操作`, visible: true, prop: 'operate' }
])
// 表单、搜索参数、规则
const { form, queryParams, rules, curAvailableQty } = toRefs(data)
// 当时实例
const { proxy } = getCurrentInstance()

/**
 * 验证数值是否合法
 * @param {数值} value 
 */
function validQty(rule, value, callback) {
    if (value > curAvailableQty.value) {
        callback(new Error(`不可超过可用数量${curAvailableQty.value}`));
    } else {
        callback()
    }
}

/**
 * 重置操作表单
 */
function reset() {
    form.value = {
        skuId: undefined,
        spuCode: undefined,
        spuName: undefined,
        skuCode: undefined,
        origLocationId: undefined,
        origWarehousName: undefined,
        origLocationCode: undefined,
        destLocationId: undefined,
        destWarehousName: undefined,
        destLocationCode: undefined,
        qty: undefined,
        ownerId: undefined
    }
    curAvailableQty.value = 0
    proxy.resetForm('moveRef')
}

/**
 * 获取列表
 */
function getList() {
    loading.value = true
    let params = proxy.addDateRange(queryParams.value, dateRange.value)
    getAllInfo(params).then(res => {
        loading.value = false
        moveList.value = res.data.result
        total.value = res.data.totalNum
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
    title.value = "库存移动"
    open.value = true
}

/**
 * 导出按钮操作 
 */
function handleExport() {
    proxy.$modal
        .confirm('是否确认导出所有仓库移动信息数据项?', '警告', {
            confirmButtonText: '确定',
            cancelButtonText: '取消',
            type: 'warning'
        })
        .then(async () => {
            await exportAllInfo()
        })
}

/**
 * 提交按钮
 */
function submitForm() {
    proxy.$refs['moveRef'].validate((valid) => {
        if (valid) {
            addInfo(form.value).then(res => {
                proxy.$modal.msgSuccess('新增成功')
                open.value = false
                getList()
            })
        }
    })
}

/**
 * 商品编码输入框点击函数
 */
function spuCodeClick() {
    reset()
    stockSelectOpen.value = true
}

/**
 * 目标库位输入框点击函数
 */
function destLocationCodeClick() {
    locationSelectOpen.value = true
}

/**
 * 取消按钮
 */
function cancel() {
    open.value = false
    reset()
}

/**
 * 库存选择框返回数据函数
 * @param {数据} val 
 */
function dialogStockData(val) {
    form.value.skuId = val.skuId
    form.value.skuCode = val.skuCode
    form.value.spuCode = val.spuCode
    form.value.spuName = val.spuName
    form.value.origLocationId = val.locationId
    form.value.origLocationCode = val.locationCode
    form.value.origWarehousName = val.warehouseName
    form.value.ownerId = val.ownerId

    curAvailableQty.value = val.qty
}

/**
 * 库位选择框返回数据函数
 * @param {数据} val 
 */
function dialogLocationData(val) {
    form.value.destLocationId = val.locationId
    form.value.destLocationCode = val.locationCode
    form.value.destWarehousName = val.warehouseName
}

// /**
//  * 查看按钮操作 
//  * @param {行数据} row 
//  */
// function handleDetail(row) {
//     let moveId = row.moveId
//     getInfo(moveId).then(res => {
//         console.log(res.data);
//     })
// }

/**
 * 确认移动按钮操作 
 * @param {行数据} row 
 */
function handleMove(row) {
    proxy.$modal
        .confirm(`是否确定移动${row.jobCode}?`)
        .then(function () {
            confirmMove(row.moveId).then(res => {
                proxy.$modal.msgSuccess('移动成功')
                getList();
            })
        })
}

/**
 * 删除按钮操作
 * @param {行数据} row 
 */
function handleDelete(row) {
    proxy
        .$modal
        .confirm(`是否确认删除${row.jobCode}的数据项？`)
        .then(() => {
            return deleteInfo(row.moveId)
        })
        .then(() => {
            getList()
            proxy.$modal.msgSuccess('删除成功')
        })
        .catch(() => { })
}

getList()
</script>

<style lang="scss" scoped >
:deep(.el-overlay-dialog) {
    overflow: hidden;
}
</style>