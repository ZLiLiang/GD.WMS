import request from '@/utils/request'
import { praseStrZero } from '@/utils/ruoyi'
import { downFile } from '@/utils/request'

const baseUrl = '/receive/asn'

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
 * 通过id查询单个asn信息
 * @param {ans信息ID} id 
 * @returns 封装请求
 */
export function getInfo(id) {
    let url = `${baseUrl}/get/${praseStrZero(id)}`
    return request({
        url: url,
        method: 'get'
    })
}

/**
 * 根据asn信息id修改asn信息
 * @param {asn信息ID} id 
 * @param {asn信息实体} entity 
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
 * 确认分拣
 * @param {行数据Id} id 
 */
export function sorted(id) {
    let url = `${baseUrl}/sorted/${praseStrZero(id)}`
    return request({
        url: url,
        method: 'put'
    })
}

/**
 * 取消卸货
 * @param {行数据Id} id 
 */
export function cancelUnload(id) {
    let url = `${baseUrl}/cancelUnload/${praseStrZero(id)}`
    return request({
        url: url,
        method: 'put'
    })
}

/**
 * 导出待分拣信息
 */
export async function exportAllInfo() {
    let url = `${baseUrl}/sortExport`
    await downFile(url)
}
