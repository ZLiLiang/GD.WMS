import request from '@/utils/request'
import { praseStrZero } from '@/utils/ruoyi'
import { downFile } from '@/utils/request'

const baseUrl = '/basic/warehouse'

/**
 * 分页查询所有数据
 * @param {查询参数} query 
 * @returns 封装请求
 */
export function getAllInfo(query) {
    return request({
        url: baseUrl,
        method: 'get',
        params: query
    })
}

/**
 * 通过id查询单个仓库信息
 * @param {仓库ID} id 
 * @returns 封装请求
 */
export function getInfo(id) {
    let url = `${baseUrl}/${praseStrZero(id)}`
    return request({
        url: url,
        method: 'get'
    })
}

/**
 * 新增仓库
 * @param {仓库实体} entity 
 * @returns 封装请求
 */
export function addInfo(entity) {
    let url = `${baseUrl}`
    return request({
        url: url,
        method: 'post',
        data: entity
    })
}

/**
 * 根据仓库id修改仓库信息
 * @param {仓库ID} id 
 * @param {仓库实体} entity 
 * @returns 封装请求
 */
export function editInfo(id, entity) {
    let url = `${baseUrl}/edit/${praseStrZero(id)}`
    return request({
        url: url,
        method: 'post',
        data: entity
    })
}

/**
 * 根据id删除仓库信息
 * @param {仓库ID} id 
 * @returns 封装请求
 */
export function deleteInfo(id) {
    let url = `${baseUrl}/${praseStrZero(id)}`
    return request({
        url: url,
        method: 'delete'
    })
}

/**
 * 导出仓库信息
 */
export async function exportAllInfo() {
    let url = `${baseUrl}/export`
    await downFile(url)
}

/**
 * 导出供应商模板
 */
export async function exportTemplate() {
    let url = `${baseUrl}/exportTemplate`
    await downFile(url)
}