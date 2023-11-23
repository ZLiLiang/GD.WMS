import request from '@/utils/request'
import { praseStrZero } from '@/utils/ruoyi'
import { downFile } from '@/utils/request'

// 查询公司信息
export function getAll(query) {
    return request({
        url: '/basic/company',
        method: 'get',
        params: query
    })
}

// 根据id查询
export function getCompanyInfo(companyId) {
    return request({
        url: '/basic/company/' + praseStrZero(companyId),
        method: 'get'
    })
}

// 根据id删除
export function deleteById(companyId) {
    return request({
        url: '/basic/company/' + praseStrZero(companyId),
        method: 'delete'
    })
}

// 新增公司信息
export function addCompanyInfo(entity) {
    return request({
        url: '/basic/company/add',
        method: 'post',
        params: entity
    })
}

// 编辑公司信息
export function editCompanyInfo(companyId, entity) {
    return request({
        url: '/basic/company/edit/' + praseStrZero(companyId),
        method: 'post',
        params: entity
    })
}

// 导出公司
export async function exportCompany() {
    await downFile('/basic/company/export')
}