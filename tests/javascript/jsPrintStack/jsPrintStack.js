//使用异常打印堆栈
function printStack(){
    try {
        throw new Error();
    } catch (e) {
        console.log("Stack:" + e.stack);
        var loc = e.stack.replace(/Error\n/).split(/\n/)[1].replace(/^\s+|\s+$/, "");
        console.log("Location: " + loc + "");
    }
}

function func3() {
    console.log('func3');
    printStack();
    
    //安装node.js运行时可以直接用console.trace();
    //console.trace();
}
function func2() {
    console.log('func2');
    func3();
}
function func1() {
    console.log('func1');
    func2();
}

//运行例子
func1();