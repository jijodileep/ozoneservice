import 'package:flutter/material.dart';

final shellScaffoldKey = GlobalKey<ScaffoldState>();

void openAppDrawer() {
  shellScaffoldKey.currentState?.openDrawer();
}
