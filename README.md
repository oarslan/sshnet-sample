# sshnet-sample

You need to edit /etc/ssh/sshd_config and add the following lines:

PermitRootLogin yes
PasswordAuthentication yes
UseLogin yes

Then restart SSH

service ssh reload
