import subprocess
import os

# Specify the list of object names to export
objects_to_export = ["ChunkA", "ChunkB", "ChunkC"]  # Modify this list as per your needs
blend_file = "../ArtSource/Proto.blend"  # Path to your .blend file
export_directory = "../Unity/Qunatra/Assets/Core/Fbx"  # Directory to save the FBX files
blender_executable = "C:\\Program Files\\Blender Foundation\\Blender 4.2\\blender.exe"  # Update this path if necessary


# Construct the command line arguments for Blender
command = [
    blender_executable,
    "--background",  # Run Blender in background mode
    "--python", os.path.abspath("export_fbx_object.py"),  # Point to the script that handles exporting
    "--",  # Separator for Blender arguments
    os.path.abspath(blend_file),  # Absolute path of the blend file
    os.path.abspath(export_directory),  # Absolute path of the export directory
] + objects_to_export  # Append object names

# Print the command for debugging
print(f"Running command: {' '.join(command)}")

# Run the Blender command
subprocess.run(command)
