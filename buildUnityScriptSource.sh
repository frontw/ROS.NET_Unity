# Compile YAMLParser, and generate Messages CSPROJ
xbuild JustBuildMessages.sln /t:rebuild

# clean "copy/paste from here" folder
rm -rf COPY_TO_UNITY_PROJECT
mkdir COPY_TO_UNITY_PROJECT
mkdir COPY_TO_UNITY_PROJECT/Resources
# copy required source directories (including generated messages)
cp -r .ros.net/ROS_Comm COPY_TO_UNITY_PROJECT/ROS_Comm/
cp -r .ros.net/tf.net COPY_TO_UNITY_PROJECT/tf.net/
cp -r .ros.net/XmlRpc_Wrapper COPY_TO_UNITY_PROJECT/XmlRpc_Wrapper
cp -r .ros.net/MeshLib COPY_TO_UNITY_PROJECT/MeshLib/
cp -r Messages COPY_TO_UNITY_PROJECT/Messages/

#CopyMeshes.bat
