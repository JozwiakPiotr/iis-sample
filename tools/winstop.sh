for vm in $(virsh list --all --name | grep "win*"); do virsh shutdown $vm; done
