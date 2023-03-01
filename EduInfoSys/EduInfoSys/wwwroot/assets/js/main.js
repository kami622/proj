let delBtns = document.querySelectorAll(".btn-delete")
delBtns.forEach(btn =>
    btn.addEventListener("click", function (e) {
        e.preventDefault()
        Swal.fire({
            title: 'Are you sure?',
            text: "You won't be able to revert this!",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Yes, delete it!'
        }).then((result) => {
            if (result.isConfirmed) {
                fetch(btn.getAttribute("href")).then(res => {
                    if (res.status == 200) {
                        location.reload()
                    }
                    else {
                        Swal.fire(
                            'Error!',
                            'Something went wrong.',
                            'error'
                        )
                    }
                })
            }
        })
    }))

let delBtn = document.querySelector(".btn-deldetails")
if (delBtn != null) {
    delBtn.addEventListener("click", function (e) {
        e.preventDefault()
        Swal.fire({
            title: 'Are you sure?',
            text: "You won't be able to revert this!",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Yes, delete it!'
        }).then((result) => {
            if (result.isConfirmed) {
                fetch(delBtn.getAttribute("href")).then(res => {
                    if (res.status == 200) {
                        location.href = document.querySelector(".goback").getAttribute("href")
                    }
                    else {
                        Swal.fire(
                            'Error!',
                            'Something went wrong.',
                            'error'
                        )
                    }
                })
            }
        })
    })
}


let quill = document.querySelector("#quill-editor")
let formatQuill = document.querySelector("#formatQuill")
if (quill != null) {
    quill.addEventListener("DOMSubtreeModified", function () {
        document.querySelector("#description").value = quill.firstChild.innerHTML
    })
}

let taskCards = document.querySelectorAll(".taskCard")
taskCards.forEach(btn => btn.addEventListener("click", function () {
    location.href = btn.querySelector(".taskName").getAttribute("href")
}))

let passwordTab = document.querySelector("#passwordTab")
let customizationTab = document.querySelector("#customizationTab")
if (passwordTab != null) {
    passwordTab.querySelector("#updateBtn").addEventListener("click", function (e) {
        e.preventDefault()
        fetch(passwordTab.querySelector("#updateForm").getAttribute("action"), {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                currentPassword: passwordTab.querySelector("#currentPassword").value,
                newPassword: passwordTab.querySelector("#newPassword").value,
                confirmPassword: passwordTab.querySelector("#confirmPassword").value
            })
        }).then(res => res.json()).then(res => {
            
            if (res != "ok") {
                window.notyf.open({
                    type: "error",
                    message: res,
                    duration: 10000,
                    ripple: true,
                    dismissible: false,
                    position: {
                        x: "center",
                        y: "bottom"
                    }
                });
            }
            else {
                passwordTab.querySelector("#currentPassword").value = ""
                passwordTab.querySelector("#newPassword").value = ""
                passwordTab.querySelector("#confirmPassword").value = ""
                window.notyf.open({
                    type: "success",
                    message: "Password has been changed",
                    duration: 10000,
                    ripple: true,
                    dismissible: false,
                    position: {
                        x: "center",
                        y: "bottom"
                    }
                });
            }
        })
    })
}
if (customizationTab != null) {
    customizationTab.querySelector("#updateBtn").addEventListener("click", function (e) {
        e.preventDefault()
        fetch(customizationTab.querySelector("#updateForm").getAttribute("action"), {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                colorScheme: customizationTab.querySelector("#colorScheme").value,
                sidebarLayout: customizationTab.querySelector("#sidebarLayout").value,
                sidebarPosition: customizationTab.querySelector("#sidebarPosition").value,
                layout: customizationTab.querySelector("#layout").value
            })
        }).then(res => res.json()).then(res => {
            if (res != "ok") {
                window.notyf.open({
                    type: "error",
                    message: res,
                    duration: 10000,
                    ripple: true,
                    dismissible: false,
                    position: {
                        x: "center",
                        y: "bottom"
                    }
                });
            }
            else {
                location.reload()
            }
        })
    })
}


let detailsBtns = document.querySelectorAll(".btn-details")

detailsBtns.forEach(btn => btn.addEventListener("click", function (e) {
    e.preventDefault()
    fetch(btn.getAttribute("href")).then(res => res.text()).then(res => {
        document.querySelector("#detailscard").innerHTML = res

        let btn = document.querySelector(".btn-userdelete")
        btn.addEventListener("click", function (e) {
            e.preventDefault()
            Swal.fire({
                title: 'Are you sure?',
                text: "You won't be able to revert this!",
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#3085d6',
                cancelButtonColor: '#d33',
                confirmButtonText: 'Yes, delete it!'
            }).then((result) => {
                if (result.isConfirmed) {
                    fetch(btn.getAttribute("href")).then(res => {
                        if (res.status == 200) {
                            location.reload()
                        }
                        else {
                            Swal.fire(
                                'Error!',
                                'Something went wrong.',
                                'error'
                            )
                        }
                    })
                }
            })
        })
    })
}))


let controlTab = document.querySelector("#controlTab")


let controlBtns = document.querySelectorAll(".subjectcontrol")
controlBtns.forEach(btn => btn.addEventListener("click", function (e) {
    e.preventDefault()
    fetch(btn.getAttribute("href")).then(res => res.text()).then(res => {
        controlTab.innerHTML = res
        let delBtns = controlTab.querySelectorAll(".btn-delete")
        delBtns.forEach(btn =>
            btn.addEventListener("click", function (e) {
                e.preventDefault()
                Swal.fire({
                    title: 'Are you sure?',
                    text: "You won't be able to revert this!",
                    icon: 'warning',
                    showCancelButton: true,
                    confirmButtonColor: '#3085d6',
                    cancelButtonColor: '#d33',
                    confirmButtonText: 'Yes, delete it!'
                }).then((result) => {
                    if (result.isConfirmed) {
                        fetch(btn.getAttribute("href")).then(res => {
                            if (res.status == 200) {
                                location.href = document.querySelector("#pageUrl") + "?page=settings"
                            }
                            else {
                                Swal.fire(
                                    'Error!',
                                    'Something went wrong.',
                                    'error'
                                )
                            }
                        })
                    }
                })
            }))
        let updateBtns = document.querySelectorAll(".btn-updateatt")
        updateBtns.forEach(btn => btn.addEventListener("click", function (e) {
            e.preventDefault()
            fetch(btn.getAttribute("href")).then(res => res.text()).then(res => document.querySelector("#updateCard").innerHTML = res).then(res => {
                let updattform = document.querySelector("#updateCard").querySelector("#updattform")
                let updattbtn = document.querySelector("#updateCard").querySelector("#updattbtn")
                updattbtn.addEventListener("click", function (e) {
                    e.preventDefault()
                    fetch(updattform.getAttribute("action"), {
                        method: "POST",
                        headers: {
                            'Content-Type': 'application/json'
                        },
                        body: JSON.stringify({ id: updattform.getAttribute("data-id"), mark: document.querySelector("#updateCard").querySelector("#mark").value, comment: document.querySelector("#updateCard").querySelector("#comment").value })
                    }).then(response => response.json()).then(response => {
                        if (response == "ok") {
                            location.href = document.querySelector("#pageUrl") + "?page=settings&control=attendance"
                        }
                        else {
                            alert(response)
                        }
                    })
                })
            })
        }))
    })
}))

if (controlTab != null) {
    if (controlTab.getAttribute("data-value") == "attendance") {
        fetch(controlBtns[0].getAttribute("href")).then(res => res.text()).then(res => {
            controlTab.innerHTML = res
            let delBtns = controlTab.querySelectorAll(".btn-delete")
            delBtns.forEach(btn =>
                btn.addEventListener("click", function (e) {
                    e.preventDefault()
                    Swal.fire({
                        title: 'Are you sure?',
                        text: "You won't be able to revert this!",
                        icon: 'warning',
                        showCancelButton: true,
                        confirmButtonColor: '#3085d6',
                        cancelButtonColor: '#d33',
                        confirmButtonText: 'Yes, delete it!'
                    }).then((result) => {
                        if (result.isConfirmed) {
                            fetch(btn.getAttribute("href")).then(res => {
                                if (res.status == 200) {
                                    location.reload()
                                }
                                else {
                                    Swal.fire(
                                        'Error!',
                                        'Something went wrong.',
                                        'error'
                                    )
                                }
                            })
                        }
                    })
                }))
            let updateBtns = document.querySelectorAll(".btn-updateatt")
            updateBtns.forEach(btn => btn.addEventListener("click", function (e) {
                e.preventDefault()
                fetch(btn.getAttribute("href")).then(res => res.text()).then(res => document.querySelector("#updateCard").innerHTML = res).then(res => {
                    let updattform = document.querySelector("#updateCard").querySelector("#updattform")
                    let updattbtn = document.querySelector("#updateCard").querySelector("#updattbtn")
                    updattbtn.addEventListener("click", function (e) {
                        e.preventDefault()
                        fetch(updattform.getAttribute("action"), {
                            method: "POST",
                            headers: {
                                'Content-Type': 'application/json'
                            },
                            body: JSON.stringify({ id: updattform.getAttribute("data-id"), mark: document.querySelector("#updateCard").querySelector("#mark").value, comment: document.querySelector("#updateCard").querySelector("#comment").value })
                        }).then(response => response.json()).then(response => {
                            if (response == "ok") {
                                location.href = document.querySelector("#pageUrl") + "?page=settings&control=attendance"
                            }
                            else {
                                alert(response)
                            }
                        })
                    })
                })
            }))
        })
    }
    else if (controlTab.getAttribute("data-value") == "tasktypes") {
        fetch(controlBtns[1].getAttribute("href")).then(res => res.text()).then(res => {
            controlTab.innerHTML = res
            let delBtns = controlTab.querySelectorAll(".btn-delete")
            delBtns.forEach(btn =>
                btn.addEventListener("click", function (e) {
                    e.preventDefault()
                    Swal.fire({
                        title: 'Are you sure?',
                        text: "You won't be able to revert this!",
                        icon: 'warning',
                        showCancelButton: true,
                        confirmButtonColor: '#3085d6',
                        cancelButtonColor: '#d33',
                        confirmButtonText: 'Yes, delete it!'
                    }).then((result) => {
                        if (result.isConfirmed) {
                            fetch(btn.getAttribute("href")).then(res => {
                                if (res.status == 200) {
                                    location.reload()
                                }
                                else {
                                    Swal.fire(
                                        'Error!',
                                        'Something went wrong.',
                                        'error'
                                    )
                                }
                            })
                        }
                    })
                }))
        })
    }
    else if (controlTab.getAttribute("data-value") == "schedule") {
        fetch(controlBtns[2].getAttribute("href")).then(res => res.text()).then(res => {
            controlTab.innerHTML = res
            let delBtns = controlTab.querySelectorAll(".btn-delete")
            delBtns.forEach(btn =>
                btn.addEventListener("click", function (e) {
                    e.preventDefault()
                    Swal.fire({
                        title: 'Are you sure?',
                        text: "You won't be able to revert this!",
                        icon: 'warning',
                        showCancelButton: true,
                        confirmButtonColor: '#3085d6',
                        cancelButtonColor: '#d33',
                        confirmButtonText: 'Yes, delete it!'
                    }).then((result) => {
                        if (result.isConfirmed) {
                            fetch(btn.getAttribute("href")).then(res => {
                                if (res.status == 200) {
                                    location.reload()
                                }
                                else {
                                    Swal.fire(
                                        'Error!',
                                        'Something went wrong.',
                                        'error'
                                    )
                                }
                            })
                        }
                    })
                }))
        })
    }
}

let groupSelect = document.querySelector("#groupSelect")
if (groupSelect != null) {
    let subjectSelect = document.querySelector("#subjectSelect")
    subjectSelect.innerHTML = ""
    fetch(subjectSelect.getAttribute("data-href"), {
        method: "POST",
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(document.querySelector("#groupSelect").value)
    }).then(res => res.json()).then(res => {
        for (let i = 0; i < res.length; i++) {
            subjectSelect.innerHTML += '<option value="' + res[i].id + '">' + res[i].name + '</option>'
        }
    })
    groupSelect.addEventListener("change", function (e) {
        let subjectSelect = document.querySelector("#subjectSelect")
        subjectSelect.innerHTML = ""
        fetch(subjectSelect.getAttribute("data-href"), {
            method: "POST",
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(document.querySelector("#groupSelect").value)
        }).then(res => res.json()).then(res => {
            for (let i = 0; i < res.length; i++) {
                subjectSelect.innerHTML += '<option value="' + res[i].id + '">' + res[i].name + '</option>'
            }
        })
    })
}