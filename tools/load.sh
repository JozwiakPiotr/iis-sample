function virg() {
    for vm in $(virsh list --all --name | grep $2); do virsh $1 $vm; done
}

function startwin() {
    virg start win-
}

function stopwin() {
    virg shutdown win-
}