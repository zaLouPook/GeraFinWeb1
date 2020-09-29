function ValidateEmail(inputText) {
    var mailformat = /^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$/;
    if (inputText.value.match(mailformat)) {
        document.Login.Email.focus();
        return true;
    }
    else {
        alert("You have entered an invalid email address!");
        document.Login.Email.focus();
        return false;
        return;
    }
}