/opt/cprocsp/bin/amd64/csptest -keyset -newkeyset -cont '\\.\HDIMAGE\test -makecert
/opt/cprocsp/bin/amd64/csptest -keyset -delete -cont '\\.\HDIMAGE\test'

/opt/cprocsp/bin/amd64/certmgr -inst -cont '\\.\HDIMAGE\test'
/opt/cprocsp/bin/amd64/certmgr -list

./certmgr -inst -store mMy -cont '\\.\HDIMAGE\test'
Certmgr 1.1 (c) "Crypto-Pro",  2007-2019.
program for managing certificates, CRLs and stores

Cannot open container