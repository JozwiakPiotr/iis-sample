# TODO: lepiej zrobić win jako prefix niż suffix np. winstart

function virg() {
    for vm in $(virsh list --all --name | grep $2); do virsh $1 $vm; done
}

function winstart() {
    virg start win-
}

function winstop() {
    virg shutdown win-
}