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
    //node.js控制台可以使用这个函数
    // console.trace();
    printStack();
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