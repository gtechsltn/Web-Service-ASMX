<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Default.aspx.vb" Inherits="_Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">

    <meta charset="utf-8">
    <title>Home</title>
    <meta name="description" content="">
    <meta name="viewport" content="width=device-width, initial-scale=1">

    <meta http-equiv="Cache-Control" content="no-cache, no-store, must-revalidate">
    <meta http-equiv="Pragma" content="no-cache">
    <meta http-equiv="Expires" content="0">

    <meta property="og:title" content="">
    <meta property="og:type" content="">
    <meta property="og:url" content="">
    <meta property="og:image" content="">

    <style>
        .container {
            margin-bottom: 5px;
        }

        .button {
            margin-bottom: 5px;
        }

        .error {
            color: red;
        }

        textarea {
            width: 500px;
            height: 200px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            Home
        </div>
        <div class="container">
            <button class="button" id="btnAjaxCallDemo">Call Ajax</button>&nbsp;
            <br />
            <input class="input" type="text" id="inputName" />
            <button class="button" id="btnAjaxCallHello">Say Hello</button>
            <span class="error" id="spanName"></span>
        </div>
        <div class="container">
            <textarea id="textareaMessage"></textarea>
        </div>
    </form>
    <script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.8.3/jquery.min.js"></script>
    <script type="text/javascript">
        $(function () {

            var $inputName = $("#inputName")
            var $spanName = $("#spanName");
            var $textareaMessage = $("#textareaMessage");

            //Trim $inputName
            var name = $.trim($inputName.val());

            $("#btnAjaxCallHello").click(function (e) {

                //Prevent Event Handling
                e.preventDefault();

                //Warning + Error
                $spanName.text('');

                //Result
                $textareaMessage.html('');

                debugger;
                if (name === '') {
                    debugger;
                    $spanName
                        .removeClass()
                        .addClass("error")
                        .css("display", "block")
                        .text('Please enter your name.')
                        ;
                    return;
                }
                $.ajax({
                    type: "POST",
                    url: "http://localhost/AdminWebService.asmx/Hello",
                    data: "{ name: '" + name + "'}",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (r) {
                        $("#textareaMessage").html(r.d);
                        alert(r.d);
                    },
                    error: function (r) {

                        $("#textareaMessage").html(r.responseText);
                        alert(r.responseText);
                    },
                    failure: function (r) {
                        $("#textareaMessage").html(r.responseText);
                        alert(r.responseText);
                    }
                });
                return false;
            });

            $("#btnAjaxCallDemo").click(function () {

                $textareaMessage.html('');

                $.ajax({
                    type: "POST",
                    url: "http://localhost/AdminWebService.asmx/HelloWorld",
                    //data: "{ name: '" + name + "', age: " + age + "}",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (r) {
                        $("#textareaMessage").html(r.d);
                        alert(r.d);
                    },
                    error: function (r) {
                        $("#textareaMessage").html(r.responseText);
                        alert(r.responseText);
                    },
                    failure: function (r) {
                        $("#textareaMessage").html(r.responseText);
                        alert(r.responseText);
                    }
                });
                return false;
            });
        });
    </script>
</body>
</html>