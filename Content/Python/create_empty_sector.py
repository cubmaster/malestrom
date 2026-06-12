import unreal

MAP_PATH = "/Game/Maps/Test/EmptySector"
DATA_DIR = "/Game/Data"
DATA_TABLE_PATH = "/Game/Data/DT_ShipStats"
ROW_NAME = "Human_Starter_Fighter"


def ensure_data_directory():
    if not unreal.EditorAssetLibrary.does_directory_exist(DATA_DIR):
        unreal.EditorAssetLibrary.make_directory(DATA_DIR)


def ensure_ship_stats_datatable():
    ensure_data_directory()

    if unreal.EditorAssetLibrary.does_asset_exist(DATA_TABLE_PATH):
        unreal.log("Iron Exiles: DT_ShipStats already exists")
        return unreal.EditorAssetLibrary.load_asset(DATA_TABLE_PATH)

    row_struct = unreal.load_object(None, "/Script/IronExiles.ShipStatsRow")
    if row_struct is None:
        unreal.log_warning(
            "Iron Exiles: ShipStatsRow struct not found — compile C++ then re-run Initialize-Content"
        )
        return None

    factory = unreal.DataTableFactory()
    factory.struct = row_struct
    asset_tools = unreal.AssetToolsHelpers.get_asset_tools()
    data_table = asset_tools.create_asset(
        "DT_ShipStats",
        DATA_DIR,
        unreal.DataTable,
        factory,
    )

    if data_table is None:
        unreal.log_warning("Iron Exiles: failed to create DT_ShipStats")
        return None

    row = unreal.ShipStatsRow()
    row.max_speed = 5000.0
    row.forward_thrust = 2500.0
    row.strafe_thrust = 1800.0
    row.rotation_rate = 90.0
    row.brake_deceleration = 1200.0
    unreal.DataTableFunctionLibrary.add_row(data_table, ROW_NAME, row)
    unreal.EditorAssetLibrary.save_loaded_asset(data_table)
    unreal.log("Iron Exiles: created DT_ShipStats with row Human_Starter_Fighter")
    return data_table


def ensure_empty_sector_map():
    if unreal.EditorAssetLibrary.does_asset_exist(MAP_PATH):
        unreal.EditorLevelLibrary.load_level(MAP_PATH)
    else:
        unreal.log("Iron Exiles: creating EmptySector test map at {}".format(MAP_PATH))
        unreal.EditorLevelLibrary.new_level(MAP_PATH)

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
    unreal.EditorLevelLibrary.spawn_actor_from_class(
        unreal.PlayerStart,
        unreal.Vector(0.0, 0.0, 500.0),
        unreal.Rotator(0.0, 0.0, 0.0),
    )

    unreal.EditorLevelLibrary.save_current_level()
    unreal.log("Iron Exiles: EmptySector map saved.")


if __name__ == "__main__":
    ensure_ship_stats_datatable()
    ensure_empty_sector_map()
