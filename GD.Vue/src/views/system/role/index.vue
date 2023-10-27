<template>
    <div class="app-container">
        <el-form :model="queryParams" ref="queryForm" v-show="showSearch" :inline="true">
            <el-form-item label="角色名称" prop="roleName">
                <el-input v-model="queryParams.roleName" placeholder="请输入角色名称" clearable @keyup.enter="handleQuery" />
            </el-form-item>
            <el-form-item label="状态" prop="status">
                <el-select v-model="queryParams.status" placeholder="角色状态" clearable>
                    <el-option label="全部" :value="-1" />
                    <el-option v-for="dict in statusOptions" :key="dict.Value" :label="dict.Label"
                        :value="dict.Value" />
                </el-select>
            </el-form-item>
            <el-form-item>
                <el-button type="primary" icon="search" @click="handleQuery">搜索</el-button>
                <el-button icon="refresh" @click="resetQuery">重置</el-button>
            </el-form-item>
        </el-form>

        <el-row :gutter="10" class="mb8">
            <el-col :span="1.5">
                <el-button type="primary" plain icon="plus" @click="handleAdd">新增</el-button>
            </el-col>
            <right-toolbar v-model:showSearch="showSearch" @queryTable="getList"></right-toolbar>
        </el-row>

        <el-table v-loading="loading" :data="roleList" highlight-current-row @selection-change="handleSelectionChange">
            <el-table-column label="编号" prop="roleId" width="80" />
            <el-table-column label="名称" prop="roleName" />
            <el-table-column label="显示顺序" prop="roleSort" />
            <el-table-column label="状态" width="90">
                <template #default="scope">
                    <el-switch v-model="scope.row.status" :active-value="0" :inactive-value="1"
                        @change="handleStatusChange(scope.row)"></el-switch>
                </template>
            </el-table-column>
            <el-table-column label="用户个数" align="center" prop="userNum" width="90" />
            <el-table-column label="创建时间" prop="createTime" width="150" />
            <el-table-column label="备注" align="center" prop="remark" width="150" :show-overflow-tooltip="true" />
            <el-table-column label="操作" align="center" width="200">
                <template #default="scope">
                    <div>
                        <el-button text icon="edit" title="编辑" @click.stop="handleUpdate(scope.row)">
                        </el-button>
                        <el-button text icon="delete" title="删除" @click.stop="handleDelete(scope.row)">
                        </el-button>

                        <el-dropdown @command="(command) => handleCommand(command, scope.row)">
                            <span class="el-dropdown-link">
                                更多
                                <el-icon class="el-icon--right">
                                    <arrow-down />
                                </el-icon>
                            </span>
                            <template #dropdown>
                                <el-dropdown-menu>
                                    <el-dropdown-item command="handleMenuScope" icon="circle-check">菜单权限</el-dropdown-item>
                                    <el-dropdown-item command="handleExportMenu" icon="download">导出菜单</el-dropdown-item>
                                </el-dropdown-menu>
                            </template>
                        </el-dropdown>
                    </div>
                </template>
            </el-table-column>
        </el-table>
        <pagination v-show="total > 0" :total="total" v-model:page="queryParams.pageNum"
            v-model:limit="queryParams.pageSize" @pagination="getList" />

        <!-- 角色菜单弹框 -->
        <zDialog title="角色权限分配" key="role" top="0vh" draggable="" v-model="showRoleScope" width="700px" @close="cancel">
            <el-form :model="form" label-width="80px">
                <el-form-item label="菜单搜索">
                    <el-input placeholder="请输入关键字进行过滤" v-model="searchText"></el-input>
                </el-form-item>
                <el-form-item label="菜单权限">
                    <el-checkbox v-model="menuExpand" @change="handleCheckedTreeExpand($event, 'menu')">展开/折叠</el-checkbox>
                    <el-checkbox v-model="menuNodeAll"
                        @change="handleCheckedTreeNodeAll($event, 'menu')">全选/全不选</el-checkbox>
                    <el-checkbox v-model="form.menuCheckStrictly"
                        @change="handleCheckedTreeConnect($event, 'menu')">父子联动</el-checkbox>
                    <el-tree class="tree-border" :data="menuOptions" show-checkbox ref="menuRef" node-key="id"
                        :check-strictly="!form.menuCheckStrictly" empty-text="加载中，请稍后" highlight-current
                        :filter-node-method="menuFilterNode"
                        :props="{ children: 'children', label: 'label', class: customNodeClass }">
                        <template #default="{ node, data }">
                            <div class="custom-tree-node">
                                <span style="float: left">{{ node.label }}</span>
                                <span style="float;right: ;margin-left: 10px;">
                                    <el-tag v-if="data.status == 1" type="danger">停用</el-tag>
                                </span>
                            </div>
                        </template>
                    </el-tree>
                    <div style="color: red">请在菜单管理里面添加对应的菜单权限</div>
                </el-form-item>
            </el-form>
            <template #footer>
                <el-button text @click="cancel">取消</el-button>
                <el-button type="primary" @click="submitDataScope">保存</el-button>
            </template>
        </zDialog>

        <!-- 添加或修改角色配置对话框 -->
        <zDialog :title="title" key="roleEdit" v-model="open" append-to-body @close="cancel">
            <el-form ref="formRef" :model="form" :rules="rules" label-width="90px">
                <el-row>
                    <el-col :lg="12">
                        <el-form-item label="角色名称" prop="roleName">
                            <el-input v-model="form.roleName" placeholder="请输入角色名称" />
                        </el-form-item>
                    </el-col>
                    <el-col :lg="12">
                        <el-form-item label="角色顺序" prop="roleSort">
                            <el-input-number v-model="form.roleSort" controls-position="right" :min="0" />
                        </el-form-item>
                    </el-col>
                    <el-col :lg="12">
                        <el-form-item label="状态">
                            <el-radio-group v-model="form.status">
                                <el-radio v-for="dict in statusOptions" :key="dict.Value" :label="parseInt(dict.Value)">{{
                                    dict.Label }}</el-radio>
                            </el-radio-group>
                        </el-form-item>
                    </el-col>
                    <el-col :lg="24">
                        <el-form-item label="备注">
                            <el-input v-model="form.remark" type="textarea" placeholder="请输入内容"></el-input>
                        </el-form-item>
                    </el-col>
                </el-row>
            </el-form>
            <template #footer>
                <el-button text @click="cancel">取消</el-button>
                <el-button type="primary" @click="submitForm">提交</el-button>
            </template>
        </zDialog>
    </div>
</template>
  
<script setup name="role">
import { listRole, getRole, delRole, addRole, updateRole, exportRole, dataScope, changeRoleStatus, exportRoleMenu } from '@/api/system/role'
import { roleMenuTreeselect } from '@/api/system/menu'

const { proxy } = getCurrentInstance()

const loading = ref(true)
// 选中数组
const ids = ref([])
// 非单个禁用
const single = ref(true)
// 非多个禁用
const multiple = ref(true)
// 显示搜索条件
const showSearch = ref(false)
// 总条数
const total = ref(0)
// 角色表格数据
const roleList = ref([])
// 弹出层标题
const title = ref('')
// 是否显示弹出层
const open = ref(false)
const menuExpand = ref(true)
const menuNodeAll = ref(false)
// 日期范围
const dateRange = ref([])
// 状态数据字典
const statusOptions = [{ Value: "0", Label: "正常" }, { Value: "1", Label: "停用" }]
// 是否显示下拉菜单分配
const showRoleScope = ref(false)
// 菜单列表
const menuOptions = ref([])
// 查询参数
const queryParams = reactive({
    pageNum: 1,
    pageSize: 10,
    roleName: undefined,
    roleKey: undefined,
    status: -1
})
const searchText = ref('')

const state = reactive({
    form: {},
    rules: {
        roleName: [{ required: true, message: '角色名称不能为空', trigger: 'blur' }],
        roleSort: [{ required: true, message: '角色顺序不能为空', trigger: 'blur' }]
    },
    defaultProps: {
        children: 'children',
        label: 'label',
        menuType: customNodeClass
    }
})
const menuRef = ref()
const formRef = ref()
const { form, rules, defaultProps } = toRefs(state)

watch(searchText, (val) => {
    proxy.$refs.menuRef.filter(val)
})

/** 查询角色列表 */
function getList() {
    loading.value = true

    listRole(proxy.addDateRange(queryParams, dateRange.value)).then((response) => {
        roleList.value = response.data.result
        total.value = response.data.totalNum
        loading.value = false
    })
}

// 所有菜单节点数据
function getMenuAllCheckedKeys() {
    // 目前被选中的菜单节点
    const checkedKeys = proxy.$refs.menuRef.getCheckedKeys()
    // 半选中的菜单节点
    const halfCheckedKeys = proxy.$refs.menuRef.getHalfCheckedKeys()
    checkedKeys.unshift.apply(checkedKeys, halfCheckedKeys)
    return checkedKeys
}

/** 根据角色ID查询菜单树结构 */
function getRoleMenuTreeselect(roleId) {
    return roleMenuTreeselect(roleId).then((response) => {
        menuOptions.value = response.data.menus
        return response
    })
}

// 角色状态修改
function handleStatusChange(row) {
    const text = row.status == '0' ? '启用' : '停用'

    proxy
        .$modal
        .confirm('确认要"' + text + '""' + row.roleName + '"角色吗?', '警告', {
            confirmButtonText: '确定',
            cancelButtonText: '取消',
            type: 'warning'
        })
        .then(function () {
            return changeRoleStatus(row.roleId, row.status)
        })
        .then(() => {
            proxy.$modal.msgSuccess(text + '成功')
        })
        .catch(function () {
            row.status = row.status == 0 ? 1 : 0
        })
}

// 取消按钮
function cancel() {
    open.value = false
    showRoleScope.value = false
    reset()
}

// 表单重置
function reset() {
    if (proxy.$refs.menuRef != undefined) {
        proxy.$refs.menuRef.setCheckedKeys([])
    }
    ; (menuExpand.value = false),
        (menuNodeAll.value = false),
        (form.value = {
            roleId: undefined,
            roleName: undefined,
            roleKey: undefined,
            roleSort: 99,
            status: 0,
            menuIds: [],
            deptIds: [],
            dataScope: 1,
            menuCheckStrictly: true,
            deptCheckStrictly: true,
            remark: undefined
        })
    proxy.resetForm('form')
}

/** 搜索按钮操作 */
function handleQuery() {
    queryParams.pageNum = 1
    getList()
}

/** 重置按钮操作 */
function resetQuery() {
    dateRange.value = []
    proxy.resetForm('queryForm')
    handleQuery()
}

// 多选框选中数据
function handleSelectionChange(selection) {
    ids.value = selection.map((item) => item.roleId)
    single.value = selection.length != 1
    multiple.value = !selection.length
}

// 更多操作触发
function handleCommand(command, row) {
    switch (command) {
        case 'handleMenuScope':
            handleMenuScope(row)
            break
        case 'handleExportMenu':
            handleExportMenu(row)
        default:
            break
    }
}

// 树权限（展开/折叠）
function handleCheckedTreeExpand(value, type) {
    if (type == 'menu') {
        const treeList = menuOptions.value
        for (let i = 0; i < treeList.length; i++) {
            proxy.$refs.menuRef.store.nodesMap[treeList[i].id].expanded = value
        }
    } else if (type == 'dept') {
        const treeList = deptOptions.value
        for (let i = 0; i < treeList.length; i++) {
            proxy.$refs.deptRef.store.nodesMap[treeList[i].id].expanded = value
        }
    }
}

// 树权限（全选/全不选）
function handleCheckedTreeNodeAll(value, type) {
    if (type == 'menu') {
        proxy.$refs.menuRef.setCheckedNodes(value ? menuOptions.value : [])
    } else if (type == 'dept') {
        proxy.$refs.deptRef.setCheckedNodes(value ? deptOptions.value : [])
    }
}

// 树权限（父子联动）
function handleCheckedTreeConnect(value, type) {
    if (type == 'menu') {
        form.value.menuCheckStrictly = !!value
    } else if (type == 'dept') {
        form.value.deptCheckStrictly = !!value
    }
}

// 菜单筛选
function menuFilterNode(value, data) {
    if (!value) return true
    return data.label.indexOf(value) !== -1
}

/** 新增按钮操作 */
function handleAdd() {
    reset()
    open.value = true
    title.value = '添加角色'
    showRoleScope.value = false
}

/** 修改按钮操作 ok */
function handleUpdate(row) {
    reset()
    showRoleScope.value = false
    const roleId = row.roleId || ids.value
    getRole(roleId).then((response) => {
        form.value = response.data
        open.value = true
        title.value = '修改角色'
    })
}

/** 分配角色权限按钮操作 */
// 新增 和上面代码基本相同
function handleMenuScope(row) {
    if (row.roleId == 1) {
        showRoleScope.value = false
        return
    }
    reset()
    showRoleScope.value = true
    const roleId = row.roleId || ids.value
    const roleMenu = getRoleMenuTreeselect(roleId)

    roleMenu.then((res) => {
        const checkedKeys = res.data.checkedKeys
        checkedKeys.forEach((v) => {
            nextTick(() => {
                proxy.$refs.menuRef.setChecked(v, true, false)
            })
        })
    })
    form.value = {
        roleId: row.roleId,
        roleName: row.roleName,
        roleKey: row.roleKey,
        menuCheckStrictly: row.menuCheckStrictly
    }
}

const router = useRouter()

/** 提交按钮 */
function submitForm() {
    proxy.$refs['formRef'].validate((valid) => {
        if (valid) {
            if (form.value.roleId != undefined && form.value.roleId > 0) {
                form.value.type = 'edit'
                updateRole(form.value).then((response) => {
                    proxy.$modal.msgSuccess('修改成功')
                    open.value = false
                    getList()
                })
            } else {
                form.value.type = 'add'
                addRole(form.value).then((response) => {
                    open.value = false
                    if (response.code == 200) {
                        proxy.$modal.msgSuccess('新增成功')
                        getList()
                    } else {
                        proxy.$modal.msgError(response.msg)
                    }
                })
            }
        }
    })
}

/** 提交按钮（菜单数据权限） */
function submitDataScope() {
    if (form.value.roleId != undefined) {
        form.value.menuIds = getMenuAllCheckedKeys()
        dataScope(form.value).then((response) => {
            proxy.$modal.msgSuccess('修改成功')
            getList()
            cancel()
        })
    } else {
        proxy.$modal.msgError('请选择角色')
    }
}

/** 删除按钮操作 */
function handleDelete(row) {
    const roleIds = row.roleId || ids.value
    proxy
        .$modal
        .confirm('是否确认删除角色编号为"' + roleIds + '"的数据项?', '警告', {
            confirmButtonText: '确定',
            cancelButtonText: '取消',
            type: 'warning'
        })
        .then(function () {
            return delRole(roleIds)
        })
        .then(() => {
            getList()
            proxy.$modal.msgSuccess('删除成功')
        })
}

/** 导出按钮操作 */
function handleExport() {
    proxy
        .$modal
        .confirm('是否确认导出所有角色数据项?', '警告', {
            confirmButtonText: '确定',
            cancelButtonText: '取消',
            type: 'warning'
        })
        .then(function () {
            return exportRole(queryParams)
        })
        .then((response) => {
            proxy.download(response.data.path)
        })
}
// 导出角色菜单
function handleExportMenu(row) {
    proxy.$modal
        .confirm('是否确认导出所有角色菜单数据项?', '警告', {
            confirmButtonText: '确定',
            cancelButtonText: '取消',
            type: 'warning'
        })
        .then(async () => {
            await exportRoleMenu({ roleId: row.roleId })
        })
}

getList()

function customNodeClass(data, node) {
    if (data.menuType == 'C') {
        return 'tree-item-flex'
    }
    return null
}
</script>
<style scoped>
/* tree border */
.tree-border {
    margin-top: 5px;
    border: 1px solid #e5e6e7;
    background: var(--base-bg-main) none;
    border-radius: 4px;
    width: 100%;
    height: 400px;
    overflow-y: auto;
}

.el-dropdown-link {
    cursor: pointer;
    color: var(--el-color-primary);
    display: flex;
    align-items: center;
}

.el-dropdown {
    vertical-align: middle;
}
</style>
  