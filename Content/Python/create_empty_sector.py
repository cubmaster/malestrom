import unreal

MAP_PATH = "/Game/Maps/Test/EmptySector"

if __name__ == "__main__":
    unreal.log("Iron Exiles: creating EmptySector test map at {}".format(MAP_PATH))
    unreal.EditorLevelLibrary.new_level(MAP_PATH)

    # Basic lighting for PIE
    unreal.EditorLevelLibrary.spawn_actor_from_class(
        unreal.DirectionalLight,
        unreal.Vector(0.0, 0.0, 5000.0),
        unreal.Rotator(-45.0, 0.0, 0.0),
    )
    unreal.EditorLevelLibrary.spawn_actor_from_class(
        unreal.SkyLight,
        unreal.Vector(0.0, 0.0, 0.0),
        unreal.Rotator(0.0, 0.0, 0.0),
    )

    unreal.EditorLevelLibrary.save_current_level()
    unreal.log("Iron Exiles: EmptySector map saved.")
