#!/usr/bin/env python
#-* coding:UTF-8 -*


import os
import pymysql
from pymysql.cursors import DictCursor
from jinja2 import Template
import json
import sys
reload(sys)
sys.setdefaultencoding('utf8')

SKIPED_FILE_TAG = '[local no use]'

HERE = os.path.dirname(os.path.abspath(__file__))
ROOT_DIR = HERE

MODEL_DIR = os.path.join(ROOT_DIR, '../Scripts/ConfigVo')
if not os.path.exists(MODEL_DIR):
    os.makedirs(MODEL_DIR)

MODEL_TEMPLATE_FILE1 = os.path.join(HERE, "model1.tpl")
MODEL_TEMPLATE_FILE2 = os.path.join(HERE, "model2.tpl")

CONFIG_FILE = os.path.join(ROOT_DIR, "config.json")


def to_camel_case(snake_str):
    components = snake_str.split('_')
    first_char = components[0][0]
    if first_char in ('0', '1', '2', '3', '4', '5', '6', '7', '8', '9'):
        first_char = 'A' + first_char
    name = first_char.upper() + components[0][1:] + \
        "".join(x.title() for x in components[1:])
    return name


go_type_dict = {
    'smallint': 'int',
    'tinyint': 'int',
    'varchar': 'string',
    'int': 'int',
    'decimal': 'float',
    'timestamp': 'Int64',
    'bigint': 'Int64',
    'char': 'string',
    'float': 'float',
    'text': 'string',
    'longtext': 'string',
    'date': 'string',
}


def get_go_type(db_name, db_type):
    #if (db_type == "int" and db_name.endswith('time')) or db_name == "v":
    #   return 'Int64'
    return go_type_dict.get(db_type)


def render(conn, db_name, model_dir):
    cur = conn.cursor()
    sql = """ SELECT TABLE_NAME,TABLE_COMMENT FROM TABLES
    WHERE TABLE_SCHEMA = %s AND TABLE_NAME = %s
    """
    tempTable = 'mail_info'
    cur.execute(sql, [db_name,tempTable])
    result = cur.fetchall()

    if result:
        if not os.path.exists(model_dir):
            os.mkdir(model_dir)
        os.system("rm %s/*" % model_dir)
    all_render_dict = {'table_list': []}

    with open(MODEL_TEMPLATE_FILE1, "r") as f:
        tpl1 = f.read()
    with open(MODEL_TEMPLATE_FILE2, "r") as f:
        tpl2 = f.read()
    # for table_mao
    table_list = []

    for row in result:
        table_name = row['TABLE_NAME']
        table_comment = row['TABLE_COMMENT']

        if SKIPED_FILE_TAG in table_comment:
            print 'skip table name ' + table_name
            continue

        sql = """
        SELECT
        COLUMN_NAME,DATA_TYPE, COLUMN_COMMENT,
        COLUMN_DEFAULT,COLUMN_KEY,EXTRA
        FROM COLUMNS
        WHERE TABLE_NAME = %s  and TABLE_SCHEMA = %s
        """

        cur.execute(sql, [table_name, db_name])

        columns = cur.fetchall()

        struct_name = to_camel_case(table_name)
        column_list = []
        primaryKey_list = []
        primaryKeyCnt = 0
        for c in columns:
            column_name = c['COLUMN_NAME']
            field_name = to_camel_case(column_name)
            column_type = c['DATA_TYPE']
            go_type = get_go_type(column_name, column_type)
            column_comment = c['COLUMN_COMMENT']
            primary_key = c['COLUMN_KEY']
            is_PrimaryKey = bool(c['COLUMN_KEY'] == "PRI")
            if is_PrimaryKey:
                primaryKeyCnt = primaryKeyCnt +1
            column_list.append({
                'field_name': field_name,
                'type': go_type,
                'name': column_name,
                'comment': column_comment.strip(),
                'is_language':'[Language]' in column_comment.strip(),
                'is_PrimaryKey':is_PrimaryKey,
            })

        render_dict = {
            'table_name': table_name,
            'struct_name': struct_name,
            'column_list': column_list,
        }
        if primaryKeyCnt > 1:
            template = Template(tpl2)
        else:
            template = Template(tpl1)
        model_file_path = os.path.join(model_dir, '%s.cs' % (to_camel_case(table_name)+'Vo'))
        with open(model_file_path, 'w') as f:
            f.write(template.render(**render_dict))
    cur.close()
    conn.close()


def run():
    with open(CONFIG_FILE, "r") as f:
        config = json.loads(f.read())
    print "config:", config

    base_db_conn = pymysql.connect(host=config['db_host'],
                                   port=config['db_port'],
                                   charset='utf8',
                                   user=config['db_username'],
                                   passwd=config['db_password'],
                                   db='information_schema', cursorclass=DictCursor)
    render(base_db_conn, config['db_name'],  MODEL_DIR)

if __name__ == "__main__":
    run()