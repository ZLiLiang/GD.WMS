<template>
    <div class="app-container">
        <!-- 搜索功能 -->
        <el-form :model="queryParams" ref="queryRef" v-show="showSearch" :inline="true">
            <el-form-item label="供应商名称" prop="supplierName">
                <el-input v-model="queryParams.supplierName" placeholder="请输入供应商名称" clearable style="width: 160px" />
            </el-form-item>
            <el-form-item label="规格名称" prop="skuName">
                <el-input v-model="queryParams.skuName" placeholder="请输入规格名称" clearable style="width: 160px" />
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
        <el-table v-loading="loading" :data="asnNoticeList" highlight-current-row>
            <el-table-column type="selection" width="55" align="center" />
            <el-table-column label="到货通知书编号" prop="asnNo" />
            <el-table-column label="商品编码" prop="spuCode" />
            <el-table-column label="商品名称" prop="spuName" />
            <el-table-column label="规格编码" prop="skuCode" />
            <el-table-column label="规格名称" prop="skuName" />
            <el-table-column label="货主名称" prop="ownerName" />
            <el-table-column label="供应商名称" prop="supplierName" />
            <el-table-column label="到货通知书数据" prop="asnQty" />
            <el-table-column label="总重量" prop="weight" />
            <el-table-column label="总体积" prop="volume" />
            <el-table-column label="创建人" prop="createBy" v-if="columns.showColumn('createBy')" />
            <el-table-column label="创建时间" prop="createTime" v-if="columns.showColumn('createTime')" />
            <el-table-column label="操作人" prop="updateBy" v-if="columns.showColumn('updateBy')" />
            <el-table-column label="操作时间" prop="updateTime" v-if="columns.showColumn('updateTime')" />
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
        <el-dialog :title="title" v-model="open" width="450px" :show-close="false" :draggable="true">
            <el-form :model="form" :rules="rules" ref="asnNoticeRef" label-width="120px">
                <el-form-item label="供应商名称" prop="supplierId">
                    <el-select v-model="form.supplierId" placeholder="请选择供应商名称" clearable style="width: 100%;">
                        <el-option v-for="item in supplierOptions" :key="item.supplierId" :label="item.supplierName"
                            :value="item.supplierId" />
                    </el-select>
                </el-form-item>
                <el-form-item label="货主名称" prop="ownerId">
                    <el-select v-model="form.ownerId" placeholder="请选择货主名称" clearable style="width: 100%;">
                        <el-option v-for="item in ownerOptions" :key="item.ownerId" :label="item.ownerName"
                            :value="item.ownerId" />
                    </el-select>
                </el-form-item>
                <el-form-item label="商品编码" prop="spuCode">
                    <el-input v-model="form.spuCode" placeholder="请输入商品编码" clearable @click="inputClick" :readonly="true" />
                </el-form-item>
                <el-form-item label="商品名称" prop="spuName">
                    <el-input v-model="form.spuName" placeholder="请输入商品名称" clearable :readonly="true" />
                </el-form-item>
                <el-form-item label="规格编码" prop="skuCode">
                    <el-input v-model="form.skuCode" placeholder="请输入规格编码" clearable :readonly="true" />
                </el-form-item>
                <el-form-item label="规格名称" prop="skuName">
                    <el-input v-model="form.skuName" placeholder="请输入规格名称" clearable :readonly="true" />
                </el-form-item>
                <el-form-item label="到货通知书数据" prop="asnQty">
                    <el-input v-model="form.asnQty" placeholder="请输入到货通知书数据" clearable />
                </el-form-item>
            </el-form>

            <template #footer>
                <el-button text @click="cancel">取消</el-button>
                <el-button type="primary" @click="submitForm">提交</el-button>
            </template>
        </el-dialog>

        <z-SkuSelectDialog v-model:visible="skuSelectOpen" @dialogData="dialogData" />
    </div>
</template>

<script setup>
import { getAllInfo, getSupplierOptions, getOnwerOptions, addInfo, editInfo, getInfo, deleteInfo, exportAllInfo } from '@/api/receive/notice'

// 总条数
const total = ref(0)
// 展示搜索界面
const showSearch = ref(false)
// 加载...
const loading = ref(true)
// 通知列表
const asnNoticeList = ref([])
// 供应商选择列表
const supplierOptions = ref([])
// 货主选择列表
const ownerOptions = ref([])
// 展示对话框
const open = ref(false)
// 商品选择框
const skuSelectOpen = ref(false)
// 对话框标题
const title = ref('')
// 时间范围
const dateRange = ref([])
// 数据
const data = reactive({
    form: {
        supplierId: undefined,
        ownerId: undefined,
        spuId: undefined,
        spuCode: undefined,
        spuName: undefined,
        skuId: undefined,
        skuCode: undefined,
        skuName: undefined,
        asnQty: undefined
    },
    queryParams: {
        pageNum: 1,
        pageSize: 10,
        supplierName: undefined,
        skuName: undefined
    },
    rules: {
        supplierId: [{ required: true, message: '供应商名称不能为空', trigger: 'blur' }],
        ownerId: [{ required: true, message: '货主名称不能为空', trigger: 'blur' }],
        asnQty: [{ message: '到货通知书数据只能为数字', trigger: 'blur', pattern: /^[0-9]+$/ }]
    }
})
// 列显隐信息
const columns = ref([
    { key: 0, label: `创建人`, visible: false, prop: 'createBy' },
    { key: 1, label: `创建时间`, visible: false, prop: 'createTime' },
    { key: 2, label: `操作人`, visible: false, prop: 'updateBy' },
    { key: 3, label: `操作时间`, visible: false, prop: 'updateTime' },
    { key: 4, label: `操作`, visible: true, prop: 'operate' }
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
        supplierId: undefined,
        ownerId: undefined,
        spuId: undefined,
        spuCode: undefined,
        spuName: undefined,
        skuId: undefined,
        skuCode: undefined,
        skuName: undefined,
        asnQty: undefined
    }
    proxy.resetForm('asnNoticeRef')
}

/**
 * 获取列表
 */
function getList() {
    loading.value = true
    let params = proxy.addDateRange(queryParams.value, dateRange.value)
    getAllInfo(params).then(res => {
        loading.value = false
        asnNoticeList.value = res.data.result
        total.value = res.data.totalNum
    })
    getSupplierOptions().then(res => {
        supplierOptions.value = res.data
    })
    getOnwerOptions().then(res => {
        ownerOptions.value = res.data
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
    open.value = true
    title.value = "新增到货通知"
}

/**
 * 导出按钮操作 
 */
function handleExport() {
    proxy.$modal
        .confirm('是否确认导出所有到货信息数据项?', '警告', {
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
    proxy.$refs['asnNoticeRef'].validate((valid) => {
        if (valid) {
            if (form.value.asnId != undefined) {
                editInfo(form.value.asnId, form.value).then(res => {
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
 * 输入点击
 */
function inputClick() {
    skuSelectOpen.value = true
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
    open.value = true
    title.value = "编辑到货通知"
    getInfo(row.asnId).then(res => {
        form.value = res.data
    })
}

/**
 * 删除按钮操作
 * @param {行数据} row 
 */
function handleDelete(row) {
    const asnId = row.asnId
    proxy.$modal
        .confirm('是否确认删除"' + row.asnNo + '"的数据项？')
        .then(function () {
            return deleteInfo(asnId)
        })
        .then(() => {
            getList()
            proxy.$modal.msgSuccess('删除成功')
        })
}

/**
 * 商品选择框返回数据函数
 * @param {数据} val 
 */
function dialogData(val) {
    delete val.supplierId
    for (let key in form.value) {
        if (key === 'supplierId' || key === 'ownerId' || key === 'asnQty') {
            continue
        }
        form.value[key] = val[key]
    }
}

getList()
</script>

<style></style>