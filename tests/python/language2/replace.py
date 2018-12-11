#!/usr/bin/python
#-* coding:UTF-8 -*

import json
import csv
import os
import codecs
import sys
reload(sys)
sys.setdefaultencoding('utf-8')

abspath = os.path.abspath(sys.argv[0])
dname = os.path.dirname(abspath)
os.chdir(dname)
    
# 翻译好的文件放到这个文件夹下
dataPath = dname + "\\translateData\\"
# 旧的文件
oldFilePath = dname + "\\oldFiles\\"

# 创建新的文件目录
def createFile():
    path = dname + "\\newFiles\\"
    if not os.path.exists(path):
        os.mkdir(path)
    os.system("del /S/Q %s" % path)
    return path

def replaceTranslate(path, fileName):
    print("path===>>", path)
    print("fileName====>>>", fileName)
    # fileName = "skill_effect.txt"
    # 翻译过的
    srcFile = codecs.open(dataPath + fileName, 'r', 'utf-8')
    s_reader = csv.reader(srcFile, delimiter=',',quotechar='', quoting=csv.QUOTE_NONE)
    # 表名字段
    srcTitle =s_reader.next()

    # 被替换的文件
    destFile = codecs.open(oldFilePath + fileName, 'r', 'utf-8')
    d_reader = csv.reader(destFile, delimiter=',',quotechar='', quoting=csv.QUOTE_NONE)
    # 表名字段
    destTitle = d_reader.next()
    
    # 获取dest字段位置
    def getPos(key):
        for id in range(len(srcTitle)):
            title =destTitle[id]
            if title == key:
                return id
    # 翻译过的关联字段
    s_keypos = []
    # 翻译过的翻译字段
    s_datapos = []
    # 没翻译的关联字段
    d_keypos = []
    # 没翻译的翻译字段
    d_datapos = []
    for id in range(len(srcTitle)):
        title =srcTitle[id]
        print("title===>", title)
        pos = title.find("$")
        if pos != -1:
            s_keypos.append(id)
            pos = getPos(title.strip('$'))
            d_keypos.append(pos)
        else:
            s_datapos.append(id)
            pos = getPos(title)
            d_datapos.append(pos)

    sw = csv.reader(srcFile, delimiter=',',quotechar='"', quoting=csv.QUOTE_ALL)
    sourceData = []
    for row in sw:
        sourceData.append(row)

    dw = csv.reader(destFile, delimiter=',',quotechar='"', quoting=csv.QUOTE_ALL)
    destData = []
    for row in dw:
        destData.append(row)
    
    # 替换
    newFile = codecs.open(path+fileName, 'w', 'utf-8')
    h = csv.writer(newFile,  delimiter = ',', quotechar='', quoting=csv.QUOTE_NONE)
    # 写入表名
    h.writerow(destTitle)
    w = csv.writer(newFile, delimiter = ',', quotechar = '"', quoting = csv.QUOTE_ALL)
    
    def compare(sp,dp, sd,dd):
        for v in range(len(sp)):
            s = sp[v]
            t = dp[v]
            if sd[s] != dd[t]:
                return False
        return True
    def compare2(destId):
        destVal = sourceData[destId]
        for v in range(len(sourceData)):
            if sourceData[v] == destVal:
                 return True
        return False

    def replace(skp,dkp, sd,dd):
        for v in range(len(skp)):
            s = skp[v]
            t = dkp[v]
            dd[t] = sd[s]
        return dd
    print("destData len===>", len(destData))
    print("sourceData len===>", len(sourceData))
    # 逐行判断
    for id in range(len(destData)):
        print("id===>", id)
        # dval = destData[id]
        # sVal = sourceData[id]
        # if compare(s_keypos, d_keypos, sVal, dval):
        if compare2(id):
            val = replace(s_datapos, d_datapos, sVal, dval)
            w.writerow(val)
            
    srcFile.close()
    destFile.close()
    newFile.close()

# replaceTranslate()

def main():
    path = createFile()
    files = os.listdir(dataPath)
    # print files
    for v in files:
        replaceTranslate(path, v)

main()
