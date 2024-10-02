import os
import bpy
import sys

class ExportFbxObject:
    def __init__(self, blend_file, export_path, object_name):
        self.blend_file = blend_file
        self.export_path = export_path
        self.object_name = object_name

    def open_blend_file(self):
        bpy.ops.wm.open_mainfile(filepath=self.blend_file)

    def deselect_all(self):
        bpy.ops.object.select_all(action="DESELECT")

    def find_object(self):
        match_objects = [obj for obj in bpy.data.objects if self.object_name == obj.name]
        if not match_objects:
            print(f"ERROR: Object '{self.object_name}' not found.")
            return None
        
        if len(match_objects) > 1:
            print(f"WARNING: Found more than 1 object named '{self.object_name}'. Exporting the first one.")

        return match_objects[0]

    def select_object_and_children(self, target_object):
        bpy.context.view_layer.objects.active = target_object
        target_object.select_set(True)
        bpy.ops.object.select_grouped(extend=True, type='CHILDREN_RECURSIVE')

    def export(self):
        print("Blender version:", bpy.app.version_string)

        self.open_blend_file()
        self.deselect_all()

        target_object = self.find_object()
        if not target_object:
            return  # If object is not found, return early

        self.select_object_and_children(target_object)

        bpy.ops.export_scene.fbx(
            filepath=self.export_path,
            path_mode='RELATIVE',
            use_custom_props=True,
            use_selection=True,
            apply_scale_options='FBX_SCALE_UNITS',
            object_types={'EMPTY', 'MESH'}
        )

        print(f"Exported {target_object.name} and its children to {self.export_path}")

    @staticmethod
    def export_objects(blend_file, objects_to_export, export_dir):
        if not os.path.exists(export_dir):
            os.makedirs(export_dir)

        for obj_name in objects_to_export:
            export_path = os.path.join(export_dir, f"{obj_name}.fbx")
            exporter = ExportFbxObject(blend_file, export_path, obj_name)
            exporter.export()

# Main execution for blender
if __name__ == "__main__":
    if len(sys.argv) < 4:
        print("Usage: <blend_file> <export_directory> <objects_to_export>")
        sys.exit(1)

    args_after_double_dash = sys.argv[sys.argv.index('--') + 1:] if '--' in sys.argv else []
    if args_after_double_dash:
        blend_file = args_after_double_dash[0]  # Path to your .blend file
        export_directory = args_after_double_dash[1]  # Directory to save the FBX files
        objects_to_export = args_after_double_dash[2:]  # Object names to export
        # Call the export function with the parameters
        ExportFbxObject.export_objects(blend_file, objects_to_export, export_directory)
