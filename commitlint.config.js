/*
 * @Description: Copyright (c) ydfk. All rights reserved
 * @Author: ydfk
 * @Date: 2021-09-16 14:48:25
 * @LastEditors: ydfk
 * @LastEditTime: 2021-09-16 20:28:51
 */
module.exports = {
  ignores: [(commit) => commit.includes("init")],
  extends: ["gitmoji"],
};
