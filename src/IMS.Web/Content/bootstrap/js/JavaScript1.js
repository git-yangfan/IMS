Modal.prototype.adjustBody_beforeShow = function () {
    var body_scrollHeight = $('body')[0].scrollHeight;
    var docHeight = document.documentElement.clientHeight;
    if (body_scrollHeight > docHeight) {
        $('body').css({
            'overflow': 'hidden',
            'margin-right': '15px'
        });
        $('.modal').css({ 'overflow-y': 'scroll' })
    } else {
        $('body').css({
            'overflow': 'auto',
            'margin-right': '0'
        });
        $('.modal').css({ 'overflow-y': 'auto' })
    }
}
Modal.prototype.adjustBody_afterShow = function () {
    var body_scrollHeight = $('body')[0].scrollHeight;
    var docHeight = document.documentElement.clientHeight;
    if (body_scrollHeight > docHeight) {
        $('body').css({
            'overflow': 'auto',
            'margin-right': '0'
        });
    } else {
        $('body').css({
            'overflow': 'auto',
            'margin-right': '0'
        });
    }
}