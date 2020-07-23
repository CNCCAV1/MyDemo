// ==UserScript==
// @name         北京职业技能刷视频
// @namespace    http://tampermonkey.net/
// @version      0.1
// @description  try to take over the world!
// @author       You
// @match        https://www.bjjnts.cn/lessonStudy/6/152
// @run-at       document-idle
// ==/UserScript==

// 禁止右键
document.oncontextmenu = noMenuOne;
document.onkeydown = function () {
    if (window.event && window.event.keyCode == 123) {
        event.keyCode = 0;
        event.returnValue = false;
        return false;
    }
};
function noMenuOne() {
    return false;
}
// 选择需要观察变动的节点
const targetNode = document;

// 观察器的配置（需要观察什么变动）
const config = { attributes: true, childList: true, subtree: true };

// 当观察到变动时执行的回调函数
const callback = function (mutationsList, observer) {
    // Use traditional 'for loops' for IE 11
    layer.close(layer.index);
};

// 创建一个观察器实例并传入回调函数
const observer = new MutationObserver(callback);

// 以上述配置开始观察目标节点
observer.observe(targetNode, config);

const USERMEDIA_MAP = {
    not_suppeort: 400, // 当前网络协议不支持
    already_used: 401, // 摄像头或者麦克风已经被占用
    not_forbidden: 403, // 用户拒绝（或者之前拒绝过）摄像头或者麦克风的使用权限
    without_camera: 404, // 当前设备未安装网络摄像头
    error_constraint: 405, // 当前无法被满足的constraint对象
    empty_constraint: 406, // constraints对象未设置
    unknown: 500, // 未知错误
}

var lessonNum = 1;
var my_course = parseInt("1");
var is_supported_userMedia = 200;
var is_login = parseInt("1");
var duration = parseInt("409");
var courseid = parseInt("6");
var lessonid = parseInt("152");
var maxTime = parseInt("409");
var learnDuration = parseInt("409");
var currentTime = parseInt("409");
var url = "https://cm15-c110-2.play.bokecc.com/flvs/ca/QxIJo/uofWklYehN-10.mp4?t=1594720547&amp;key=2DB1262ABF5E2339BE09A71BC4079166&amp;tpl=10";
var isface = parseInt("0");
var faceTime = [];
var faceSignTime = 0;
var faceSign = 0;
var beginFaceSign = 0;
var oldTime = 0;

var video = document.getElementById("studymovie");
$(function () {
    layui.use(['form','layer'], function () {
        var form = layui.form;
        var layer = layui.layer;
        // 禁止手机
        var sUserAgent= navigator.userAgent.toLowerCase();
        var bIsIpad= sUserAgent.match(/ipad/i) == "ipad";
        var bIsIphoneOs= sUserAgent.match(/iphone os/i) == "iphone os";
        var bIsMidp= sUserAgent.match(/midp/i) == "midp";
        var bIsUc7= sUserAgent.match(/rv:1.2.3.4/i) == "rv:1.2.3.4";
        var bIsUc= sUserAgent.match(/ucweb/i) == "ucweb";
        var bIsAndroid= sUserAgent.match(/android/i) == "android";
        var bIsCE= sUserAgent.match(/windows ce/i) == "windows ce";
        var bIsWM= sUserAgent.match(/windows mobile/i) == "windows mobile";
        if (bIsIpad || bIsIphoneOs || bIsMidp || bIsUc7 || bIsUc || bIsAndroid || bIsCE || bIsWM) {
            $("#studymovie").attr("src","");
            return layer.alert('请在电脑端观看点播视频', { icon: 5 });
        }

        $.post("/lessonStudy/" + courseid + "/" + lessonid, function(data){
            url = data.data.url;
        });

        // 视频播放
        $(".change_chapter").click(function () {
            check_network_protocol_support();
            var lock = $(this).data('lock');
            if(lock == 1) return;
            $(this).parent(".course_study_sonmenu").addClass("on")
            $(this).parents("li").siblings().find(".course_study_sonmenu").removeClass("on")
            $(this).parent(".course_study_sonmenu").siblings().removeClass("on")
            lessonNum = $(this).data('lessonnum');
            lessonid = $(this).data('lessonid');
            isface = $(this).data('isface');
            faceTime = $(this).data('facetime');
            learnDuration = parseInt($(this).data('learnduration'));
            maxTime = parseInt($(this).data('learnduration'));
            faceSign = 0;
            faceSignTime = 0;
            currentTime = 0;
            oldTime = 0;

            $.post("/lessonStudy/" + courseid + "/" + lessonid, function(data){
                url = data.data.url;
                $("#studymovie").attr('src', url);
                video.play();
            });
        })

        video.onloadedmetadata = function () {
            var vLength = video.duration;
            duration = parseInt(vLength);
            $.post("/editVideoChapter/" + courseid + "/" + lessonid, { duration: duration });
        };
        // 禁止视频右键
        $('.course_study_movie').bind('contextmenu', function () {
            return false;
        });

        if(my_course == 1){
            if(currentTime == 0 && lessonNum == 1){
                check_network_protocol_support();
                beginFaceSign = 1;
                video.pause();
                canvas();
            }

            // 禁止视频快进
            video.addEventListener('timeupdate', function (e) {
                console.log("禁止视频快进事件执行");
                var currentTime = parseInt(video.currentTime);
                if (currentTime - maxTime > 1 && currentTime > learnDuration) {
                    video.currentTime = maxTime;
                    currentTime = maxTime;
                }
                if (currentTime > maxTime) {
                    maxTime = currentTime;
                }
                if(currentTime > faceSignTime){
                    if(isface == 1){
                        $.each(faceTime, function(i, n){
                            if(faceSign == i && currentTime == n){
                                video.pause();
                                canvas();
                            }
                        });
                        if(currentTime > faceTime[faceSign] && faceSignTime < faceTime[faceSign]){
                            video.pause();
                        }
                    }else if(isface == 2){
                        $.each(faceTime, function(i, n){
                            if(faceSign == i && currentTime == n){
                                video.pause();
                                layer.alert('请确认是否继续观看？', { icon: 3, closeBtn: 0 }, function (index) {
                                    faceSign++;
                                    faceSignTime = currentTime;
                                    layer.close(index);
                                    video.play();
                                })
                            }
                        });
                        if(currentTime > faceTime[faceSign] && faceSignTime < faceTime[faceSign]){
                            video.pause();
                        }
                    }
                }
                if(beginFaceSign == 1 && currentTime > 0) video.pause();
                if(currentTime - oldTime >= 60 && currentTime > learnDuration){
                    $.post("/addstudentTaskVer2/" + courseid + "/" + lessonid, { "learnTime": currentTime });
                    oldTime = currentTime;
                }
            })

            // 播放事件
            video.addEventListener('play', function (e) {
                document.querySelector('video').playbackRate = 4.0;
                console.log("播放事件执行");
            })

            // 暂停监听
            video.addEventListener('pause', function (e) {
                currentTime = parseInt(video.currentTime)
            })
            video.addEventListener('seeked', function(e) {
                currentTime = parseInt(video.currentTime)
            })

            // 播放结束
            video.addEventListener('ended', function (e) {
                console.log("播放结束事件执行");
                if (duration - maxTime > 1 && duration != learnDuration) {
                    video.currentTime = maxTime;
                    return;
                }
                $('.lesson-'+lessonNum+' .course_study_menuschedule').html('已完成<br>100%');
                var nextLesson = lessonNum+1;
                $('.lesson-'+nextLesson+' .course_study_menuschedule').html('已完成<br>0%');
                $('.lesson-'+nextLesson).attr('data-lock','0');
                learnTime = duration;
                $('.lesson-' + nextLesson).click();
                $.cookie('lessonid', lessonid);
                $.post("/addstudentTaskVer2/" + courseid + "/" + lessonid, { "learnTime": duration });
            })
        }
    });
})

// 兼容性检测 Tinywan: https://blog.csdn.net/weixin_30607029/article/details/101174970
function is_supported_get_userMedia() {
    navigator.getUserMedia = navigator.getUserMedia || navigator.webkitGetUserMedia || navigator.mozGetUserMedia;
    constraints = { audio: false, video: {width: { ideal: 470 },height: { ideal: 470 }} };
    if (navigator.getUserMedia) {
        navigator.mediaDevices.getUserMedia(constraints).then(function success(stream) {
        }).catch(function (err) {
            if (err.name == 'NotFoundError' || err.name == 'DevicesNotFoundError'){
                is_supported_userMedia = USERMEDIA_MAP.without_camera;
            } else if(err.name == 'NotReadableError' || err.name == 'TrackStartError'){
                is_supported_userMedia = USERMEDIA_MAP.already_used;
            } else if(err.name == 'OverconstrainedError' || err.name == 'ConstraintNotSatisfiedErrror'){
                is_supported_userMedia = USERMEDIA_MAP.error_constraint;
            } else if(err.name == 'NotAllowedError' || err.name == 'PermissionDeniedError'){
                is_supported_userMedia = USERMEDIA_MAP.not_forbidden;
            } else if(err.name == 'TypeError' || err.name == 'TypeError'){
                is_supported_userMedia = USERMEDIA_MAP.empty_constraint;
            }
        });
    } else {
        is_supported_userMedia = USERMEDIA_MAP.not_suppeort;
    }
};

function check_network_protocol_support() {
    is_supported_get_userMedia();
    if (is_supported_userMedia == USERMEDIA_MAP.not_suppeort) {
        video.pause();
        layer.alert('请使用谷歌(Chrome)或火狐(Firefox)浏览器', { icon: 5 });
        return false;
    } else if (is_supported_userMedia == USERMEDIA_MAP.without_camera) {
        video.pause();
        layer.alert('当前设备未安装网络摄像头', { icon: 5 });
        return false;
    } else if (is_supported_userMedia == USERMEDIA_MAP.already_used) {
        video.pause();
        layer.alert('摄像头或者麦克风已经被占用', { icon: 5 });
        return false;
    } else if (is_supported_userMedia == USERMEDIA_MAP.error_constraint) {
        video.pause();
        layer.alert('太高的帧速率或者高的分辨率', { icon: 5 });
        return false;
    } else if (is_supported_userMedia == USERMEDIA_MAP.not_forbidden) {
        video.pause();
        layer.alert('用户拒绝（或者之前拒绝过）摄像头或者麦克风的使用权限', { icon: 5 });
        return false;
    } else if (is_supported_userMedia == USERMEDIA_MAP.empty_constraint) {
        video.pause();
        layer.alert('当传递给getUserMedia()的约束对象为空或者将所有轨道', { icon: 5 });
        return false;
    } else if (is_supported_userMedia == USERMEDIA_MAP.unknown) {
        video.pause();
        layer.alert('未知错误', { icon: 5 });
        return false;
    }
    return true;
};

function canvas(){
    try {
        facerecogn()
    }
    catch (e) {
        layer.alert('该浏览器不支持人脸识别,请选择最新版谷歌或者火狐浏览器', { icon: 5 });return false
    }
}
function facerecogn(){
    $('.face_recogn').show()
    var canvas = document.getElementById("canvas"),
        context = canvas.getContext('2d'),
        videoCanvas = document.getElementById("video"),
        start = document.getElementById("face_startbtn"),
        mediaStreamTrack;
    mediaConstraints = {
        audio: false,
        video: {
            width: { ideal: 470 },
            height: { ideal: 470 },
            frameRate: {ideal: 10},
            facingMode: "user"
        }
    };
    errBack = function (error) {
        console.log("Video capture error: ", error.code);
    };
    //navigator.getUserMedia这个写法在Opera中好像是navigator.getUserMedianow
    //更新兼容火狐浏览器
    if (navigator.mediaDevices.getUserMedia || navigator.getUserMedia || navigator.webkitGetUserMedia || navigator.mozGetUserMedia) {
        navigator.getUserMedia = navigator.getUserMedia || navigator.webkitGetUserMedia || navigator.mozGetUserMedia;
        navigator.getUserMedia(mediaConstraints, function (stream) {
            mediaStreamTrack = typeof stream.stop === 'function' ? stream : stream.getTracks()[0];
            videoCanvas.srcObject = stream;
            videoCanvas.play();
        }, errBack);
    }
    //开始检测
    start.addEventListener('click', function() {
        let num = 10
        var time = '';
        var that = $('.face_startbtn')
        that.addClass('process')
        that.attr('disabled',true)
        that.children('span').text('检测中...')
        that.children('em').text(num)

        context.drawImage(videoCanvas, 0, 0, 280, 280);
        var imageBase64 = canvas.toDataURL();
        $.ajax({
            type: "POST",
            url: "/faceVerifyBase64",
            data: { "image_file": imageBase64 },
            datatype: "json",
            success: function (res) {
                time = setInterval(function(){
                    num --;
                    that.children('em').text(num)
                    if(num == 0){
                        clearInterval(time);
                        that.addClass('again')
                        that.removeAttr("disabled");
                        that.children('span').text('重新验证');
                        that.children('em').text('')
                    }
                },1000);
                if (res.code == 200) {
                    clearInterval(time);
                    $('.result_tip').text('');
                    $('.success').show();
                    var face_time = setTimeout(function() {
                        $('.face_recogn').hide();
                        $('.success').hide();
                        that.removeClass('process')
                        that.removeClass('again')
                        that.removeAttr("disabled");
                        that.children('em').text('')
                        that.children('span').text('开始检测');
                        mediaStreamTrack && mediaStreamTrack.stop();

                        $("#studymovie").attr('src', url);
                        if(beginFaceSign == 0){
                            faceSignTime = currentTime;
                            faceSign++;
                        }else{
                            beginFaceSign = 0;
                        }
                        video.currentTime = currentTime;
                        video.play();

                    }, 2000);
                } else {
                    $('.result_tip').text(res.msg);
                }
            },
            error: function () {
                $('.result_tip').text('检测失败');
            }
        });
    }, false);
}